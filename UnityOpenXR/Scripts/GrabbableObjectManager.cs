using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using static UnityVR.LibraryForVRTextbook;

namespace UnityVR{
    //このスクリプがアタッチされたオブジェクトにXRGrabInteractable コンポーネントを必ず持たせるようにします。もしアタッチされていない場合、Unityが自動的に追加
    [RequireComponent(typeof(XRGrabInteractable))]

    public class GrabbableObjectManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI displayMessage;

        bool isReady;
        //XRGrabInteractable コンポーネントへの参照を保持
        XRGrabInteractable grabInteractable;
        //オブジェクトのメッシュレンダラーコンポーネントへの参照を保持
        MeshRenderer meshRenderer;
        Color normalColor;
        static readonly Color ColorOnActivated = Color.red;

        void Awake()
        {
            if(displayMessage is null){Application.Quit();}

            //ゲームオブジェクトにアタッチされているXRGrabInteractableコンポーネントを取得
            grabInteractable = GetComponent<XRGrabInteractable>();
            //ゲームオブジェクトにアタッチされているMeshRendererコンポーネントを取得
            meshRenderer = GetComponent<MeshRenderer>();
            if(grabInteractable is null || meshRenderer is null || !meshRenderer.enabled)
            {
                isReady = false;
                var errorMessage = "#grabInteractable or #MeahREnderer";
                displayMessage.text = $"{GetSourceFileName()}\r\nError:{errorMessage}";
                return;
            }

            isReady = true;
        }

        void OnEnable()
        {
            if(!isReady){return;}

            normalColor = meshRenderer.material.color;

            //オブジェクトが選択されたときOnSelectEnteredメソッドを呼び出す
            grabInteractable.selectEntered.AddListener(OnSelectEntered);
            //オブジェクトの選択が解除されたときOnSelectExitedメソッドを呼び出す
            grabInteractable.selectExited.AddListener(OnSelectExited);
            //オブジェクトがアクティブ化されたとき、OnSelectExitedメソッドを呼び出す
            grabInteractable.activated.AddListener(OnActivated);
            //オブジェクトが非アクティブ化されたとき、OnDeactivatedメソッドを呼び出す
            grabInteractable.deactivated.AddListener(OnDeactivated);
        }

        void OnSelectEntered(SelectEnterEventArgs args)=>displayMessage.text=$"{GetCallerMember()}\r\n";

        void OnSelectExited(SelectExitEventArgs args)
        {
            displayMessage.text = $"{GetCallerMember()}\r\n";
            meshRenderer.material.color = normalColor;
        }

        void OnActivated(ActivateEventArgs args)
        {
            displayMessage.text = $"{GetCallerMember()}\r\n";
            meshRenderer.material.color = ColorOnActivated;
        }

        void OnDeactivated(DeactivateEventArgs args)
        {
            displayMessage.text = $"{GetCallerMember()}\r\n";
            meshRenderer.material.color = normalColor;
        }

    }



}

