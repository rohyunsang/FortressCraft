using Fusion;

namespace Agit.FortressCraft
{
	public static class NetworkRunnerStaticRefs
	{
		public static LevelManager GetLevelManager(this NetworkRunner runner) => runner ? (LevelManager)runner.SceneManager : null;
	}
}