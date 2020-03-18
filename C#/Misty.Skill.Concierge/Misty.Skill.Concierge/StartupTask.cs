using MistyRobotics.SDK.Messengers;
using Windows.ApplicationModel.Background;

namespace Misty.Skill.Concierge
{
    public sealed class StartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            RobotMessenger.LoadAndPrepareSkill(taskInstance, new MistyNativeSkill(), MistyRobotics.Common.Types.SkillLogLevel.Verbose);
        }
    }
}