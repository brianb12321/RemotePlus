using RemotePlusLibrary.SubSystem.Command.CommandClasses;

namespace RemotePlusLibrary.SubSystem.Command
{
    public delegate CommandResponse CommandDelegate(CommandRequest request, CommandPipeline pipeline, ICommandEnvironment currentEnvironment);
}