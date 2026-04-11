using ColbyO.Untitled.Player;
using ColbyO.Untitled.Traffic;
using InteractionSystem;
using PlazmaGames.Core;
using PlazmaGames.Core.Debugging;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

namespace ColbyO.Untitled.MonoSystems
{
    public class GameLogicMonoSystem : MonoBehaviour, IGameLogicMonoSystem
    {
        private Scheduler _scheduler = new Scheduler();
        private HashSet<string> _inRange = new HashSet<string>();
        private HashSet<string> _triggers = new HashSet<string>();
        private bool _started = false;

        private IDialogueMonoSystem _dialogueMs;

        private static class Refs
        {
            /*Act 1*/
            public static Transform PlayerCarDriverSeatLoc;
            public static Transform PlayerCarCameraTarget;
            public static SplineFollower PlayerCarController;
            public static EngineSound PlayerCarAudio;
            public static List<GameObject> PlayerCarMirrorCameras = new List<GameObject>();

            public static Interactable PlayerCarDoor;
            public static Interactable CameraInteractable;

            public static SplineContainer TrafficSpline;

            public static Transform GetOutOfCarLoc;

        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
            SceneManager.sceneUnloaded -= OnSceneUnload;
        }

        private void Start()
        {
            _dialogueMs = GameManager.GetMonoSystem<IDialogueMonoSystem>();

            Refs.PlayerCarDriverSeatLoc = GameObject.FindWithTag("Act1_PlayerCarDriverSeatLoc").transform;
            Refs.PlayerCarCameraTarget = GameObject.FindWithTag("Act1_PlayerCarCameraTarget").transform;

            GameObject playerCar = GameObject.FindWithTag("Act1_PlayerCarController");
            Refs.PlayerCarController = playerCar.GetComponent<SplineFollower>();
            Refs.PlayerCarAudio = playerCar.GetComponent<EngineSound>();
            GameObject.FindGameObjectsWithTag("Act1_PlayerCarMirrorCamera", Refs.PlayerCarMirrorCameras);

            Refs.PlayerCarDoor = GameObject.FindWithTag("Act1_PlayerCarDoor").GetComponent<Interactable>();
            Refs.CameraInteractable = GameObject.FindWithTag("Act1_CameraInteractable").GetComponent<Interactable>();

            Refs.TrafficSpline = GameObject.FindWithTag("TrafficLanes").GetComponent<SplineContainer>();

            Refs.GetOutOfCarLoc = GameObject.FindWithTag("Act1_GetOutOfCarLoc").transform;

        }

        private void Update()
        {
            _scheduler.Tick(Time.deltaTime);
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {

        }

        private void OnSceneUnload(Scene scene)
        {

        }

        public void TriggerEvent(string eventName)
        {
            PlazmaDebug.Log("Event Triggered", eventName, Color.green);

            switch (eventName)
            {
                case "Act1":
                    UTGameManager.LockMovement = false;

                    UTGameManager.PlayerMoveController.Attach(Refs.PlayerCarController.transform);
                    UTGameManager.PlayerMoveController.FreezeJustMovement();
                    UTGameManager.PlayerMoveController.DisableChacaterController();
                    UTGameManager.PlayerMoveController.TeleportTo(Refs.PlayerCarDriverSeatLoc.position);
                    UTGameManager.PlayerViewController.ToggleView(
                        PlayerViewType.Fixed, 
                        offsetOverride: new Vector3(0f, -1.58f, 0f),
                        fixedTarget: Refs.PlayerCarCameraTarget
                    );

                    UTGameManager.PlayerAnimationController.SetFlag("InDriverSeat", true);
                    UTGameManager.PlayerAnimationController.SetFlag("IsParked", false);

                    UTGameManager.PlayerWalkingAudio.Enabled = false;
                    UTGameManager.GetMonoSystem<IInventoryMonoSystem>().TakeItem("Camera");

                    Refs.PlayerCarDoor.CanInteract = false;
                    Refs.CameraInteractable.CanInteract = true;
                    Refs.CameraInteractable.gameObject.SetActive(true);
                    Refs.CameraInteractable.GetAction<TakeAction>().IsEnabled = false;

                    Refs.PlayerCarAudio.SetRpmAndThrottle(250f, 0f);

                    Refs.PlayerCarController.Initialize(Refs.TrafficSpline, 2, 30f)
                    .Then(_ =>
                    {
                        Refs.PlayerCarAudio.SetRpmAndThrottle(0f, 0f);
                        UTGameManager.PlayerAnimationController.SetFlag("IsParked", true);
                        foreach (GameObject cam in Refs.PlayerCarMirrorCameras) cam.SetActive(false);

                        Promise dialoguePromise = GameManager.GetMonoSystem<IDialogueMonoSystem>().StartDialoguePromise("Act1_Arrival", passive: true);

                        return dialoguePromise;
                    })
                    .Then(_ =>
                    {
                        Refs.CameraInteractable.GetAction<TakeAction>().IsEnabled = true;
                        return _scheduler.When(() => GameManager.GetMonoSystem<IInventoryMonoSystem>().HasItem("Camera"));
                    })
                    .Then(_ =>
                    {
                        Refs.PlayerCarDoor.CanInteract = true;
                        return Refs.PlayerCarDoor.GetAction<CarGetOutAction>().WaitForDoorToOpen();
                    })
                    .Then(_ =>
                    {
                        UTGameManager.PlayerAnimationController.SetFlag("InDriverSeat", false);

                        Promise playerMovePromise = UTGameManager.PlayerMoveController.TransitionTo(Refs.GetOutOfCarLoc, 1f);

                        Promise cameraLerpPromise = UTGameManager.PlayerViewController.TransitionView(
                            PlayerViewType.ThirdPerson,
                            1f
                        );

                        return Promise.All(playerMovePromise, cameraLerpPromise);
                    })
                    .Then(_ =>
                    {
                        UTGameManager.PlayerWalkingAudio.Enabled = true;
                        UTGameManager.PlayerMoveController.Deattach();
                        UTGameManager.PlayerMoveController.UnfreezeJustMovement();
                        UTGameManager.PlayerMoveController.EnableChacaterController();
                        Refs.PlayerCarDoor.GetAction<CarGetOutAction>().Door.Close();
                    });

                    break;
            }
        }

        public void Trigger(string triggerName)
        {
            _triggers.Add(triggerName);
        }

        public void SetInRange(string rangeName, bool state)
        {
            if (state) _inRange.Add(rangeName);
            else _inRange.Remove(rangeName);

            switch (rangeName)
            {
                default: break;
            }
        }

        private bool IsTriggered(string triggerName) => _triggers.Remove(triggerName);

        private bool IsInRange(string rangeName) => _inRange.Contains(rangeName);
    }
}