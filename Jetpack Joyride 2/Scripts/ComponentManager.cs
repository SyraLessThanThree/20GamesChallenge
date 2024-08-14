using Godot;
using System;
using Godot.Collections;

public partial class ComponentManager : Node
{
	public void ConnectSignal(GodotObject obj, String signalName, Action action)
	{
		GD.Print("Connected");
		Action localAction = new Action(action);
		//obj.Connect(signalName, Callable.From(() => {localAction.Invoke();}));
		ThisIsStupid(obj, signalName, action);
	}

	private void ThisIsStupid(GodotObject obj, String signalName, Action action)
	{
		if( !( obj.HasSignal(signalName) || obj.HasUserSignal(signalName))) return;
		//GD.Print(obj.GetSignalList());
		int argsNum = -1;
		foreach (var signal in obj.GetSignalList())
		{
			if (signal["name"].As<string>().Equals(signalName))
			{
				argsNum = signal["args"].As<Array<Dictionary>>().Count;
			}
		}

		switch (argsNum)
		{
			case 0:
				obj.Connect(signalName, Callable.From(() => {action.Invoke();}));
				break;
			case 1:
				obj.Connect(signalName, Callable.From((GodotObject obj) => {action.Invoke();}));
				break;
			case 2:
				obj.Connect(signalName, Callable.From((GodotObject obj, GodotObject obj2) => {action.Invoke();}));
				break;
			case 3:
				obj.Connect(signalName, Callable.From((GodotObject obj, GodotObject obj2, GodotObject obj3) => {action.Invoke();}));
				break;
			case 4:
				obj.Connect(signalName, Callable.From((GodotObject obj, GodotObject obj2, GodotObject obj3, GodotObject obj4) => {action.Invoke();}));
				break;
			case 5:
				obj.Connect(signalName, Callable.From((GodotObject obj, GodotObject obj2, GodotObject obj3, GodotObject obj4, GodotObject obj5) => {action.Invoke();}));
				break;
			default:
				GD.PushWarning("Couldnt Create Signal");
				break;
		}
	}
}
