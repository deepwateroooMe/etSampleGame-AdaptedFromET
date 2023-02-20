using CommandLine;
using System;
using System.Collections.Generic;
namespace ET {
    public enum AppType { // 这里，应用的类型简单了很多，没有那么多不同的服务器，注册登录服，网关服等待
        Server,
        Robot,
        Watcher, // 每台物理机一个守护进程，用来启动该物理机上的所有进程
        GameTool,
    }
    public class Options {
        [Option("AppType", Required = false, Default = AppType.Server, HelpText = "serverType enum")]
        public AppType AppType { get; set; }
        [Option("Process", Required = false, Default = 1)]
        public int Process { get; set; } = 1;
        
        [Option("Develop", Required = false, Default = 1, HelpText = "develop mode, 0正式 1开发 2压测")]
        public int Develop { get; set; } = 1;
        [Option("LogLevel", Required = false, Default = 2)]
        public int LogLevel { get; set; } = 2;
        
        [Option("Console", Required = false, Default = 0)]
        public int Console { get; set; } = 0;
        
        // 进程启动是否创建该进程的scenes
        [Option("CreateScenes", Required = false, Default = 1)]
        public int CreateScenes { get; set; } = 1;
    }
}