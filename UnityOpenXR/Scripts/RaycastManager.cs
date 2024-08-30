using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using static UnityVR.LibraryForVRTextbook;

namespace UnityVR{
    public class RaycastManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI displayMessage;
        [SerializeField] XRRayInteractor rightRay;
        [SerializeField] AudioClip soundEffect;

        bool isReady;

        void Awake()
        {
            if(displayMessage is null)
            {
                Application.Quit();
            }

            if(rightRay is null || soundEffect is null)
            {
                isReady = false;
                var errorMessage = "#rightRay or #soundEffect";
                displayMessage.text = $"{GetSourceFileName()}\r\nError:{errorMessage}";
                return;
            }

            isReady = true;
        }

        void OnEnable()
        {
            if(!isReady){return;}

            //オブジェクトを強制的に掴む動作を無効にするための設定
            rightRay.useForceGrab = false;
            //レイが衝突する最も近いオブジェクトのみを検出
            rightRay.hitClosestOnly = true;

            //イベントリスナーの追加

            //レイがオブジェクトにホバーしたとき
            rightRay.hoverEntered.AddListener(OnHoverEntered);
            //ホバーから外れたとき
            rightRay.hoverExited.AddListener(OnHoverExited);
            //選択したとき
            rightRay.selectEntered.AddListener(OnSelectEntered);
            //選択を解除したとき
            rightRay.selectExited.AddListener(OnSelectExited);

            //オブジェクトを選択したときにハプティクス（振動）を再生する設定を有効
            rightRay.playHapticsOnSelectEntered = true;
            //ハプティクスの強度を設定します
            rightRay.hapticSelectEnterIntensity = 1f;
            //ハプティクスの持続時間を設定
            rightRay.hapticSelectEnterDuration = 0.5f;

            //オブジェクトを選択したときにオーディオクリップを再生する設定を有効
            rightRay.playAudioClipOnSelectEntered = true;
            //再生するオーディオクリップを soundEffect に設定
            rightRay.audioClipForOnSelectEntered = soundEffect;
        }

        void OnDisable()
        {
            if(!isReady){return;}

            //イベントリスナーの削除
            rightRay.hoverEntered.RemoveListener(OnHoverEntered);
            rightRay.hoverExited.RemoveListener(OnHoverExited);
            rightRay.selectEntered.RemoveListener(OnSelectEntered);
            rightRay.selectExited.RemoveListener(OnSelectExited);
        }

        //引数として渡されたイベントデータとイベントリスナーの名前を使用して、操作を行うメソッド
        void DisplayInteractions<T>(T args,string EventListenerName)where T : BaseInteractionEventArgs
        {
            displayMessage.text = $"{EventListenerName}\r\n";

            if(rightRay.hasHover)//righRay が何かにホバーしていたら
            {
                //args.interactableObject を XRGrabInteractable 型にキャスト
                var grabInteractable = args.interactableObject as XRGrabInteractable;
                //grabInteractable が null でない場合、その名前を grabInteractableName に代入
                var grabInteractableName = grabInteractable != null ? grabInteractable.name:"Unknown";
                //displayMessage のテキストに、レイキャストを発生させた rightRay の名前（rightRay.name）と、インタラクト可能なオブジェクトの名前（grabInteractableName）を追加
                displayMessage.text += $"> Interactor:{rightRay.name}\r\n" + $"> Interactable:{grabInteractableName}\r\n";
            }
            
            //現在の3Dレイキャストヒット情報を取得できる場合に実行
            //現在のレイキャストのヒット情報を hit 変数に代入し、成功した場合は true を返します。
            //displayMessage のテキストに、レイキャストがヒットした位置（hit.point）を追加
            if(rightRay.hasSelection && rightRay.TryGetCurrent3DRaycastHit(out var hit)){displayMessage.text += $">Hit Position:{hit.point}\r\n";}
        }

        //ホバーイベント発生時にDisplayInteractions メソッドにイベントデータ args とイベントリスナーの名前を渡す
        void OnHoverEntered(HoverEnterEventArgs args)=>DisplayInteractions(args,GetCallerMember());
        //ホバー終了時にDisplayInteractions メソッドにイベントデータ args とイベントリスナーの名前を渡す
        void OnHoverExited(HoverExitEventArgs args)=>DisplayInteractions(args,GetCallerMember());
        //オブジェクト選択時にDisplayInteractions メソッドにイベントデータ args とイベントリスナーの名前を渡す
        void OnSelectEntered(SelectEnterEventArgs args)=>DisplayInteractions(args,GetCallerMember());
        //オブジェクトの選択解除時にDisplayInteractions メソッドにイベントデータ args とイベントリスナーの名前を渡す
        void OnSelectExited(SelectExitEventArgs args)=>DisplayInteractions(args,GetCallerMember());
    }
}

