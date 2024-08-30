//Meta Quest2のコントローラーのトリガーを引いた時に処理をさせる
//まずは、InputActionを作成→p.52ページ参照(VRプログラミング)
//フィールドの設定は、p.78ページ参照(VRプログラミング)
using UnityEngine;
using UnityEngine.InputSystem;
//UnityVR.LibraryForVRTextbookのメンバーをインポート→クラス名なしで直接使える
using static UnityVR.LibraryForVRTextbook;

namespace UnityVR
{
    public class ActionToAxisForLiftUp : ActionToControl
    {
    // Start is called before the first frame update
        [SerializeField] GameObject targetObject;

        Vector3 initPos;

        void Start()
        {
            if(targetObject is null)
            {
                isReady = false;
                errorMessage += "#targetObject";
            }

            if(!isReady)
            {
                displayMessage.text = $"{GetSourceFileName()}\r\nError:{errorMessage}";
                return;
            }

            //targetObjectの現在の位置を取得
            initPos = targetObject.transform.position;
        }

        protected override void OnActionPerformed(InputAction.CallbackContext ctx) => UpdateValue(ctx);
        protected override void OnActionCanceled(InputAction.CallbackContext ctx) => UpdateValue(ctx);

        void UpdateValue(InputAction.CallbackContext ctx)
        {
            // コールバックコンテキストから浮動小数点数値を読み取ります。これは入力デバイスから取得した値
            var liftUpValue = ctx.ReadValue<float>();
            var pos = targetObject.transform.position;
            pos.y = initPos.y = liftUpValue;
            targetObject.transform.position = pos;
            displayMessage.text = $"Lift Up:{liftUpValue:F2}";
        }

    }
}

