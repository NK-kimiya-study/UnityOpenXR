//Meata Quest2のコントローラーのボタンが押されたとき対象のオブジェクトの色を変更
////まずは、InputActionを作成→p.52ページ参照(VRプログラミング)
//フィールドの設定
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityVR.LibraryForVRTextbook;

namespace UnityVR
{
    public class ActionToButtonForChangeColor : ActionToControl
    {
        [SerializeField] GameObject targetObject;

        Renderer meshRenderer;
        Color normalColor;
        static readonly Color ColorOnPressed = Color.red;
        // Start is called before the first frame update
        void Start()
        {
            if(targetObject is null || (meshRenderer = targetObject.GetComponent<MeshRenderer>()) is null)
            {
                isReady = false;
                errorMessage += "#targetObject";
            }

            if(!isReady)
            {
                displayMessage.text = $"{GetSourceFileName()}\r\nError:{errorMessage}";
                return;

                normalColor = meshRenderer.material.color;
            }
        }

        protected override void OnActionPerformed(InputAction.CallbackContext ctx) => UpdateValue(ctx);
        protected override void OnActionCanceled(InputAction.CallbackContext ctx) => UpdateValue(ctx);

        void UpdateValue(InputAction.CallbackContext ctx)
        {
            //ボタンの状態（押されているかどうか）を読み取る
            var isOn = ctx.ReadValueAsButton();
            meshRenderer.material.color = isOn ? ColorOnPressed : normalColor;
            displayMessage.text = $"Change Color: {isOn}";
        }
    }
}

