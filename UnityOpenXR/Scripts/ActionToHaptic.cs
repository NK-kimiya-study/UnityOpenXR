using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.OpenXR.Input;
using static UnityVR.LibraryForVRTextbook;

namespace UnityVR
{
    public class ActionToHaptic : ActionToControl
    {
        //入力アクションを参照するフィールド
        [SerializeField] InputActionReference hapticActionReference;
        //実際の入力アクションを格納するフィールド
        InputAction hapticAction;

        void Start()
        {
            if(hapticActionReference is null || (hapticAction = hapticActionReference.action) is null)
            {
                isReady = false;
                errorMessage += "#hapticActionReference";
            }

            if(!isReady)
            {
                displayMessage.text = $"{GetSourceFileName()}\r\nError:{errorMessage}";
            }

            hapticAction.Enable();
        }

        protected override void OnActionPerformed(InputAction.CallbackContext ctx) => UpdateValue(ctx);
        protected override void OnActionCanceled(InputAction.CallbackContext ctx) => UpdateValue(ctx);

        void UpdateValue(InputAction.CallbackContext ctx)
        {
          //入力デバイスを取得
          //var device = ctx.action?.activeControl?.device;
          var device = ctx.action?.activeControl?.device;
          if(device is null){return;}

          var message = "Haptic:";
          //入力アクションがボタンとして扱われ、そのボタンが押されているかどうか
          if(ctx.ReadValueAsButton())//ctx.ReadValueAsButton()
          {
            // 触覚フィードバックの強度
            var intensity = 1f;
            //触覚フィードバックの持続時間
            var duration = 0.5f;
            //指定した hapticAction、強度、持続時間、およびデバイスに対して触覚フィードバックを送信
            OpenXRInput.SendHapticImpulse(hapticAction,intensity,duration,device);
            message += $"call = {ctx.action.name},haptic={hapticAction.name}\r\n device={device.name}";
          }
          displayMessage.text = message;
        }
    
}
}


