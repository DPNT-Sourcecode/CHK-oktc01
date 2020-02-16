using System.Text;

namespace BeFaster.App.Solutions.HLO
{
    public static class HelloSolution
    {
        /// <summary>
        /// Returns the Hello message with friendName
        /// </summary>
        /// <param name="friendName"></param>
        /// <returns></returns>
        public static string Hello(string friendName)
        {
            StringBuilder builder = new StringBuilder("Hello ");
            builder.Append(friendName);
            return builder.ToString();
        }
    }
}
