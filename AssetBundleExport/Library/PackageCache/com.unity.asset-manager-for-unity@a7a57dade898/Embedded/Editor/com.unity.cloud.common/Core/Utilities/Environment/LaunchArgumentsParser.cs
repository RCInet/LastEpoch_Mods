using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// This class handles launch arguments parsing of the current running process.
    /// </summary>
    class LaunchArgumentsParser
    {
        /// <summary>
        /// Holds the result of the launch arguments parsing operation as an url.
        /// </summary>
        public string ActivationUrl { get; private set; } = string.Empty;

        /// <summary>
        /// Holds the result of the launch arguments parsing operation as a string Dictionary.
        /// </summary>
        public Dictionary<string, string> ActivationKeyValues { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Parse the command line arguments given to the executable, as well as the trailing argument. The trailing
        /// argument is only non-empty if it doesn't start with '-'
        /// </summary>
        /// <param name="launchArgs">Optional override list of string to parse for launch arguments.</param>
        public LaunchArgumentsParser(string[] launchArgs = null)
        {
            launchArgs = launchArgs == null ? Environment.GetCommandLineArgs() : launchArgs;
            ParseCommandLineArguments(launchArgs);
        }

        void ParseCommandLineArguments(string[] launchArgs)
        {
            // Unity usual start command have the path to application as single argument
            // Some Build will mishandle spaces and artificially creates more than 1 argument
            int appPathLen = 0;
            int argCount = 1;
            bool appPathFound = false;
            var sb = new StringBuilder();
            for (var i = 0; i < launchArgs.Length; i++)
            {
                if(!appPathFound)
                {
                    sb.Append(launchArgs[i]);
                    appPathLen++;

                    if (File.Exists(sb.ToString()))
                    {
                        appPathFound = true;
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                    continue;
                }

                if (argCount % 2 == 1)
                {
                    if (ActivationKeyValues.ContainsKey(launchArgs[i]))
                    {
                        i++;
                        continue;
                    }
                    ActivationKeyValues.Add(launchArgs[i], string.Empty);
                }
                else
                {
                    ActivationKeyValues[launchArgs[i - 1]] = launchArgs[i];
                }
                argCount++;
            }

            // If the executable path length is less than the Args length, that means we have a trailing argument
            // We also check that it does not start with a dash, in case we want to call it with optional args such as
            // -batchmode
            var lastArg = launchArgs[launchArgs.Length - 1];
            if (appPathLen < launchArgs.Length && !lastArg.StartsWith("-") && Uri.TryCreate(lastArg, UriKind.Absolute, out _))
            {
                ActivationUrl = lastArg;
            }
        }
    }
}
