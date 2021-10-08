﻿using UnityEngine;

namespace ET
{
	public class EnterMapFinish_RemoveRoomUI: AEvent<EventType.EnterMapFinish>
	{
		protected override async ETTask Run(EventType.EnterMapFinish args)
		{
			Scene scene = args.ZoneScene;

			FUIManagerComponent fuiManagerComponent = scene.GetComponent<FUIManagerComponent>();

			FUI_RoomComponent fuiRoomComponent = fuiManagerComponent.GetFUIComponent<FUI_RoomComponent>(FUI_RoomComponent.FUIRoomName);
			
			scene.GetComponent<RoomManagerComponent>().RemoveAllRooms();
			
			FUI_RoomUtilities.RefreshRoomListBaseOnRoomData(fuiRoomComponent);
			
			scene.GetComponent<FUIManagerComponent>().Remove(FUI_RoomComponent.FUIRoomName);

			await ETTask.CompletedTask;
		}
	}
}