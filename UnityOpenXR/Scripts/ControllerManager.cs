using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using TMPro;
using static UnityVR.LibraryForVRTextbook;

namespace UnityVR
{
    public class ControllerManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI displaymessage;
        //コントローラーからのレイキャストを扱う
        [SerializeField] XRRayInteractor baseInteractor;
        [SerializeField] XRRayInteractor teleportInteractor;

        //コントローラーからの移動、回転、テレポートの有効化・無効化を参照
        [SerializeField] InputActionReference move;
        [SerializeField] InputActionReference turn;
        [SerializeField] InputActionReference teleportModeAction;
        [SerializeField] InputActionReference teleportModeCancel;

        bool isReady;

        //列挙型を定義(関連する定数をひとまとめに定義)
        enum ControllerType
        {
            Base,//ControllerType.Base
            Teleport,//ControllerType.Teleport
        }

        [Serializable]
        class ControllerMode
        {
            //コントローラーモードが有効かどうか
            public bool Enabled {get; set;}

            //特定のイベントが発生した際に実行されるメソッドを登録できるクラスの生成

            //コントローラーモードが"入った"というイベントが発生した際
            [SerializeField] UnityEvent onEntered = new();
            public UnityEvent OnEntered {get => onEntered; set => onEntered = value;}

             //コントローラーモードが更新された際
            [SerializeField] UnityEvent onUpdate = new();
            public UnityEvent OnUpdate {get => onUpdate; set => onUpdate = value;}

            //コントローラーモードから出た際
            [SerializeField] UnityEvent onExited = new();
            public UnityEvent OnExited {get => onExited; set => onExited = value;}
        }

        //Inspectorパネルでフィールドやプロパティの間にスペースを挿入するための属性
        [Space]
        //次に定義されるコントローラーモード関連のフィールドを「Controller Modes」という見出しでグループ化
        [Header("Controller Modes")]

        //ControllerModeクラスのインスタンスを生成
        [SerializeField] ControllerMode motionMode = new();
        //ControllerModeクラスのインスタンスを生成
        [SerializeField] ControllerMode interactionMode = new();
        //ControllerModeクラスのインスタンスを生成
        [SerializeField] ControllerMode teleportMode = new();

        //コントローラーモードを複数管理するためのリスト
        //motionMode、interactionMode、teleportModeなどの複数のコントローラーモードを追加して管理
        List<ControllerMode> controllerModes = new();

        void Awake()
        {   //displaymessageがnullであれば、アプリケーションを終了
            if(displaymessage is null) {Application.Quit();}

            if(baseInteractor is null || teleportInteractor is null || turn is null || move is null || teleportModeAction is null || teleportModeCancel is null)
            {
                isReady = false;
                var errorMessage = "#base/teleportInteractor or #Actions(move,etc.)";
                displaymessage.text = $"{GetSourceFileName()}\r\nError:{errorMessage}";
                return;
            }

            isReady = true;
        }

        void OnEnable()
        {
            if(!isReady) {return;}

            //各コントローラーモード（motionMode、interactionMode、teleportMode）のOnEntered、OnUpdate、OnExitedに登録されたイベントリスナーの数をチェック
            if(motionMode.OnEntered.GetPersistentEventCount() == 0
            || motionMode.OnUpdate.GetPersistentEventCount() == 0
            ||interactionMode.OnEntered.GetPersistentEventCount() == 0
            ||interactionMode.OnUpdate.GetPersistentEventCount() == 0
            ||interactionMode.OnExited.GetPersistentEventCount() == 0
            ||teleportMode.OnEntered.GetPersistentEventCount() == 0
            ||teleportMode.OnUpdate.GetPersistentEventCount() == 0
            ||teleportMode.OnExited.GetPersistentEventCount() == 0)
            {
                //少なくとも1つのイベントリスナーが登録されていない場合、isReadyをfalseに設定
                isReady = false;
                displaymessage.text = $"{GetSourceFileName()}\r\nError:#Setting EventListeners";
            }
        }

        void Start()
        {
            if(!isReady) {return;}

            //controllerModesリストに、motionMode、interactionMode、teleportModeの各コントローラーモードを追加
            controllerModes.Add(motionMode);
            controllerModes.Add(interactionMode);
            controllerModes.Add(teleportMode);

            //Baseコントローラーを設定
            SetController(ControllerType.Base);
            //状態の遷移やモードの切り替えを行うためのメソッド
            //どのモードに対して操作を行うのかを指定
            TransitionMode(null,motionMode);
        }

        void Update()
        {
           
                if(!isReady){return;}

                //controllerModesリストに含まれるすべての ControllerMode オブジェクトに対して、順番に処理
                foreach (var mode in controllerModes)
                {
                    //各 ControllerMode オブジェクトの Enabled プロパティがtrueの場合
                    if(mode.Enabled)
                    {
                        // OnUpdate イベントを実行
                        mode.OnUpdate.Invoke();
                        return;
                    }
                }
        }

        void TransitionMode(ControllerMode fromMode,ControllerMode toMode)
        {
            //切り替え前のモードが指定されている場合に以下の処理
            if(fromMode != null)
            {
                // fromMode の OnExited イベント
                fromMode.OnExited.Invoke();
                //fromMode.Enabled = false;: fromMode を無効化
                fromMode.Enabled = false;
            }

            //新しいモードが指定されている場合
            if(toMode != null)
            {
                //toMode を有効化
                toMode.Enabled = true;
                // toMode の OnEntered イベントを実行
                toMode.OnEntered.Invoke();
            }
        }

        //与えられた ControllerType に応じて、対応するコントローラーが有効化または無効化
        void SetController(ControllerType type)
        {
            baseInteractor.gameObject.SetActive(type == ControllerType.Base);
            teleportInteractor.gameObject.SetActive(type == ControllerType.Teleport);
        }

        //与えられた actionReference から InputAction を取得し、それを有効化
        void EnabledAction(InputActionReference actionReference)
        {
            var action = actionReference != null ? actionReference.action : null;
            if (action != null) {action.Enable();}
        }

        //与えられた actionReference から InputAction を取得し、それを無効化
        void  DisableAction(InputActionReference actionReference)
        {
            var action = actionReference != null ? actionReference.action : null;
            if(action != null) {action.Disable();}
        }

        ////motion モードが開始された際に実行される処理を定義
        public void OnMotionEntered()=>displaymessage.text = "Motion Mode\r\n";

        //motion モード中に毎フレーム呼び出される更新処理を定義
        public void OnMotionUpDate()
        {
            //何かを選択している場合
            if(baseInteractor.hasSelection)
            {
                //motionMode から interactionMode にモードを切り替え
                TransitionMode(motionMode,interactionMode);
                return;
            }

            var activate = teleportModeAction.action?.triggered ?? false;
            var cancel = teleportModeCancel.action?.triggered ?? false;
            //もし teleportModeAction がトリガーされ、かつ teleportModeCancel 
            //がトリガーされていない場合は、motionMode から teleportMode にモードを切り替え
            if(activate && !cancel) {TransitionMode(motionMode,teleportMode);}
        }

        //interaction モードが開始された際に実行
        public void OnInteractionEntered()
        {
            //move アクションを無効化
            DisableAction(move);
            //turn アクションを無効化
            DisableAction(turn);
            displaymessage.text = "Interaction Mode\r\n";
        }

        //interaction モード中に毎フレーム呼び出される更新処理
        public void OnInteractionUpdate()
        {

            //何も選択していない場合
            if(!baseInteractor.hasSelection)
            {
                //interactionMode から motionMode にモードを切り替え
                TransitionMode(interactionMode,motionMode);
            }
        }

        //nteraction モードが終了した際に実行される処理
        public void OnInteractionExited()
        {
            // move アクションを有効化
            EnabledAction(move);
            //turn アクションを有効化
            EnabledAction(turn);
        }

        //teleport モードが開始された際に実行
        public void OnTeleportEntered()
        {
            //コントローラーを teleport モードに設定
            SetController(ControllerType.Teleport);
            displaymessage.text = "Teleport Mode\r\n";
        }

        //teleport モード中に毎フレーム呼び出される更新処理
        public void OnTeleportUpdate()
        {

            //teleportModeCancel がトリガーされたか、または teleportModeAction 
            //がリリースされた場合は、teleportMode から motionMode にモードを切り替え
            var cancel = teleportModeCancel.action?.triggered ?? false;
            var released = teleportModeAction.action?.phase == InputActionPhase.Waiting;
            if(cancel || released){TransitionMode(teleportMode,motionMode);}
        }

        //eleport モードが終了した際に実行される処理
        //コントローラーを base モードに戻す。
        public void OnTelePortExited() => SetController(ControllerType.Base);
    }
}




