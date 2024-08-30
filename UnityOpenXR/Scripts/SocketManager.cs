using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using static UnityVR.LibraryForVRTextbook;

namespace UnityVR
{
    public class SocketManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI displayMessage;
        //XRSocketInteractorコンポーネントを参照
        [SerializeField] XRSocketInteractor socket;

        bool isReady;
        //ソケットの通常の色を保存するためのフィールド
        Color normalColor;
        //ソケットが選択されたときに使用する色を青に設定
        static readonly Color ColorOnSocketSelect = Color.blue;

        void Awake()
        {
            if(displayMessage is null){Application.Quit();}

            if(socket is null)
            {
                isReady = false;
                var errorMessage = "#socket";
                displayMessage.text = $"{GetSourceFileName()}\r\nError:{errorMessage}";
                return;

                isReady = true;
            }

        }

        void OnEnable()
        {
            if(!isReady){return;}

            socket.selectEntered.AddListener(OnSocketSelectEntered);
            socket.selectExited.AddListener(OnSocketSelectExited);
        }

        void OnDisable()
        {
            if(!isReady){return;}

            socket.selectEntered.RemoveListener(OnSocketSelectEntered);
            socket.selectExited.RemoveListener(OnSocketSelectExited);
        }

        void OnSocketSelectEntered(SelectEnterEventArgs args)
        {
            var objectInSocket = args.interactableObject as XRGrabInteractable;
            if(objectInSocket == null){return;}

            var message = $"object in Socket:{objectInSocket.name}";
            displayMessage.text = $"{GetCallerMember()}\r\n{message}\r\n";
            var meahRenderer = objectInSocket.GetComponent<MeshRenderer>();
            if(meahRenderer != null)
            {
                normalColor = meahRenderer.material.color;
                meahRenderer.material.color = ColorOnSocketSelect;
            }
        }

        void OnSocketSelectExited(SelectExitEventArgs args)
        {
            var objectInSocket = args.interactableObject as XRGrabInteractable;
            if(objectInSocket == null){return;}
            displayMessage.text = $"{GetCallerMember()}\r\n";
            var meahRenderer = objectInSocket.GetComponent<MeshRenderer>();
            if(meahRenderer != null)
            {
                meahRenderer.material.color = normalColor;
            }
        }
    }


}


