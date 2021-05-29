using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;


namespace FroggyBot.Util
{
    class EnvQuery
    {
        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMacOS() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        // public static bool HasFFmpeg =>
    }


    class CommandRunner
    {
        int ExitCode;
        Process _process;
        ProcessStartInfo ProcessInfo;

        public CommandRunner(string _command)
        {
            string[] shell = { };
            if (EnvQuery.IsWindows()) shell = new string[] { "cmd.exe", "/C " + _command };
            else if (EnvQuery.IsLinux()) shell = new string[] { "/bin/bash", " " + _command };
            else if (EnvQuery.IsMacOS()) shell = new string[] { "cmd.exe", "/C " };

            ProcessInfo = new ProcessStartInfo()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = shell[0],
                Arguments = shell[1],
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };
            _process = new Process() { StartInfo = ProcessInfo };
        }

        public int Run(bool _debug = false)
        {
            _process.StartInfo = ProcessInfo;
            bool _ = _process.Start();
            _process.WaitForExit();
            if (_debug)
            {
                string stdout = _process.StandardOutput.ReadToEnd();
                string stderr = _process.StandardError.ReadToEnd();
                Console.WriteLine(stdout);
                Console.WriteLine(stderr);
            }
            return _process.ExitCode;
        }

        public static int RunCommand(string _command, bool Debug = false)
        {
            CommandRunner c = new CommandRunner(_command);
            return c.Run(Debug);
        }

        public static int RunBinary(string name, string _command) {
            return 0;
        }
    }
}