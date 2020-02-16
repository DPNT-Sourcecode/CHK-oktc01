#!/usr/bin/env bash

set -x
set -e
set -u
set -o pipefail

SCRIPT_CURRENT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

CHALLENGE_ID=$1
CSHARP_TEST_COVERAGE_DIR="${SCRIPT_CURRENT_DIR}/coverage"
CSHARP_INSTRUMENTED_COVERAGE_REPORT="${CSHARP_TEST_COVERAGE_DIR}/instrumented-coverage.xml"
CSHARP_TEST_RUN_REPORT="${CSHARP_TEST_COVERAGE_DIR}/test-report.xml"
CSHARP_CODE_COVERAGE_INFO="${SCRIPT_CURRENT_DIR}/coverage.tdl"

exitAfterNoCoverageReportFoundError() {
  echo "No coverage report was found"
  exit -1
}

mkdir -p ${CSHARP_TEST_COVERAGE_DIR}

( cd ${SCRIPT_CURRENT_DIR} && \
     nuget restore befaster.sln )

(
    cd ${SCRIPT_CURRENT_DIR} && \
        msbuild ${SCRIPT_CURRENT_DIR}/befaster.sln                \
            /p:buildmode=debug /p:TargetFrameworkVersion=v4.5
)

[ -e ${SCRIPT_CURRENT_DIR}/__Instrumented ] && rm -fr ${SCRIPT_CURRENT_DIR}/__Instrumented
[ -e ${SCRIPT_CURRENT_DIR}/__UnitTestWithAltCover ] && rm -fr ${SCRIPT_CURRENT_DIR}/__UnitTestWithAltCover


if [[ ! -e "${SCRIPT_CURRENT_DIR}/src/BeFaster.App/Solutions/${CHALLENGE_ID}" ]]; then
   echo "" > ${CSHARP_CODE_COVERAGE_INFO}
   echo "The provided CHALLENGE_ID: '${CHALLENGE_ID}' isn't valid, aborting process..."
   exit 1
fi

# Instrument the binaries so that coverage can be collected
FULL_PATH_TO_ALTCOVER="$(cd ${SCRIPT_CURRENT_DIR} && find packages -path *altcover* | head -n 1 || true)"/tools/net45/AltCover.exe

(
    cd ${SCRIPT_CURRENT_DIR} && \
    mono ${SCRIPT_CURRENT_DIR}/${FULL_PATH_TO_ALTCOVER}                           \
      --opencover --linecover                                                     \
      --dropReturnCode                                                            \
      --inputDirectory ${SCRIPT_CURRENT_DIR}/src/BeFaster.App.Tests/bin/Debug     \
      --assemblyFilter=Adapter                                                    \
      --assemblyFilter=Mono                                                       \
      --assemblyFilter=\.Recorder                                                 \
      --assemblyFilter=Sample                                                     \
      --assemblyFilter=nunit                                                      \
      --assemblyFilter=Tests                                                      \
      --assemblyExcludeFilter=.+\.Tests                                           \
      --assemblyExcludeFilter=AltCover.+                                          \
      --assemblyExcludeFilter=Mono\.DllMap.+                                      \
      --typeFilter=System.                                                        \
      --outputDirectory=${SCRIPT_CURRENT_DIR}/__UnitTestWithAltCover              \
      --xmlReport=${CSHARP_INSTRUMENTED_COVERAGE_REPORT}
)

# Run the tests against the instrumented binaries
FULL_PATH_TO_NUNIT_CONSOLE="$(cd ${SCRIPT_CURRENT_DIR} && find packages -path *nunit*console.exe | head -n 1 || true)"

(
  cd ${SCRIPT_CURRENT_DIR} && \
    mono ${SCRIPT_CURRENT_DIR}/${FULL_PATH_TO_ALTCOVER} Runner                                          \
        --executable ${SCRIPT_CURRENT_DIR}/${FULL_PATH_TO_NUNIT_CONSOLE}                                \
        --recorderDirectory ${SCRIPT_CURRENT_DIR}/__UnitTestWithAltCover/                               \
        -w ${SCRIPT_CURRENT_DIR}                                                                        \
        -- --noheader --labels=All  --work=${SCRIPT_CURRENT_DIR}                                        \
        --result=${CSHARP_TEST_RUN_REPORT}                                                              \
        ${SCRIPT_CURRENT_DIR}/__UnitTestWithAltCover/BeFaster.App.Tests.dll || true
)

[ -e ${CSHARP_CODE_COVERAGE_INFO} ] && rm ${CSHARP_CODE_COVERAGE_INFO}

if [ -f "${CSHARP_INSTRUMENTED_COVERAGE_REPORT}" ]; then
    TOTAL_COVERAGE_PERCENTAGE=0
    COVERAGE_SUMMARY_FILE=${CSHARP_TEST_COVERAGE_DIR}/coverage-summary-${CHALLENGE_ID}.xml
    COVERAGE_IN_PACKAGE=$(xmllint ${CSHARP_INSTRUMENTED_COVERAGE_REPORT} \
                                  --xpath '//Class[starts-with(./FullName,"BeFaster.App.Solutions.'${CHALLENGE_ID}'.")]/Summary')

   echo "<xml>${COVERAGE_IN_PACKAGE}</xml>" > ${COVERAGE_SUMMARY_FILE}
   if [[ ! -z "${COVERAGE_IN_PACKAGE}" ]]; then
     COVERED=$(xmllint ${COVERAGE_SUMMARY_FILE} --xpath  'sum(//Summary/@visitedSequencePoints)')
     TOTAL_LINES=$(xmllint ${COVERAGE_SUMMARY_FILE} --xpath  'sum(//Summary/@numSequencePoints)')
     TOTAL_COVERAGE_PERCENTAGE=$(($COVERED * 100 / $TOTAL_LINES))
   fi

    echo ${TOTAL_COVERAGE_PERCENTAGE} > ${CSHARP_CODE_COVERAGE_INFO}
    cat ${CSHARP_CODE_COVERAGE_INFO}
    exit 0
else
    exitAfterNoCoverageReportFoundError
fi
