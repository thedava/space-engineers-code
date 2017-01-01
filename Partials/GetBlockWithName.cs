class GetBlockWithName {
	void Main() {
		(GridTerminalSystem.GetBlockWithName("Ore Counter Code Timer") as IMyTimerBlock);
	}

	IMyTextPanel getPanel(string panelName) {
		try {
			return (IMyTextPanel)GridTerminalSystem.GetBlockWithName(panelName);
		} catch (Exception error) {
			throw new Exception($"Unable to find panel with name '{panelName}'", error);
		}
	}
}