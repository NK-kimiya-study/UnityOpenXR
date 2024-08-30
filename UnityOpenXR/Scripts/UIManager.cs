using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityVR.LibraryForVRTextbook;

namespace UnityVR
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI displayMessage;
        [SerializeField] GameObject targetObject;
        [SerializeField] Button buttonForStart;
        [SerializeField] Slider sliderForSpeed;
        [SerializeField] TMP_Dropdown dropdownForSpeedMode;
        [SerializeField] Toggle toggleForReverse;
        [SerializeField] TMP_InputField inputFieldForSpeed;

        bool isReady;
        bool hasStarted;
        //回転速度モードのデータを格納する読み取り専用リスト
        static readonly IReadOnlyList<(string modeName,float maxSpeed)> SpeedModeData = new[]
        {
            ("Normal Mode",90f),//通常モードでの最大回転速度
            ("High Speed Mode",720f),//高速モードでの最大回転速度
        };
        float rotaionSpeed = SpeedModeData[0].maxSpeed;//現在の回転速度
        int rotaionSign = 1;//回転の方向。デフォルトで正方向（1）

        void Awake()
        {
            if(displayMessage is null){Application.Quit();}

            if(targetObject is null || buttonForStart is null || sliderForSpeed is null || dropdownForSpeedMode is null || toggleForReverse is null || inputFieldForSpeed is null)
            {
                isReady = false;
                var errorMessage = "//targetObject or //UI objects";
                displayMessage.text = $"{GetSourceFileName()}\r\nError:{errorMessage}";

                return;
            }

            isReady = true;
        }

        void OnEnable()
        {
            if(!isReady) {return;}

            //ボタンのクリックイベントリスナーを追加
            buttonForStart.onClick.AddListener(OnButtonClicked);
            //sliderForSpeed.maxValue はスライダーの最大値を設定
            sliderForSpeed.maxValue = SpeedModeData[0].maxSpeed;
            //スライダーの最小値を設定
            sliderForSpeed.minValue = rotaionSpeed;
            //スライダーの値変更イベントリスナーを追加
            sliderForSpeed.onValueChanged.AddListener(OnSliderValueChanged);
            //dropdownForSpeedMode の既存のオプションをクリア
            dropdownForSpeedMode.ClearOptions();

            //ドロップダウンに表示される選択肢が設定
            foreach (var (modeName, _) in SpeedModeData)
            {
                dropdownForSpeedMode.options.Add(new TMP_Dropdown.OptionData(modeName));
            }

            //ドロップダウンメニューの初期値を設定
            dropdownForSpeedMode.value = 0;
            //dropdownForSpeedMode の表示を更新
            dropdownForSpeedMode.RefreshShownValue();
            //dropdownForSpeedMode の値が変更されたときに OnDropdownValueChanged メソッドを呼び出す
            dropdownForSpeedMode.onValueChanged.AddListener(OnDropdownValueChanged);

            //トグルの初期状態を設定
            toggleForReverse.isOn = false;
            //トグルの値変更イベントリスナーを追加
            toggleForReverse.onValueChanged.AddListener(OnToggleValueChanged);

            //inputFieldForSpeed の内容タイプを DecimalNumber に設定
            inputFieldForSpeed.contentType = TMP_InputField.ContentType.DecimalNumber;
            //inputFieldForSpeed が選択されたときに OnInputFieldSelect メソッドを呼び出す
            inputFieldForSpeed.onSelect.AddListener(OnInputFieldSelect);
            //inputFieldForSpeed の編集が終了したときに OnInputFieldEndEdit メソッドを呼び出す
            inputFieldForSpeed.onEndEdit.AddListener(OnInputFieldEndEdit);

            //プログラムの開始状態を設定
            hasStarted = true;
            OnButtonClicked();
        }

        void OnDisable()
        {
            if(!isReady){return;}

            buttonForStart.onClick.RemoveListener(OnButtonClicked);
            sliderForSpeed.onValueChanged.RemoveListener(OnSliderValueChanged);
            dropdownForSpeedMode.onValueChanged.RemoveListener(OnDropdownValueChanged);
            toggleForReverse.onValueChanged.RemoveListener(OnToggleValueChanged);
            inputFieldForSpeed.onSelect.RemoveListener(OnInputFieldSelect);
            inputFieldForSpeed.onEndEdit.RemoveListener(OnInputFieldEndEdit);
        }

        void Update()
        {
            if(!isReady || !hasStarted){return;}

            //角速度を求める
            var angularVelocity = rotaionSign * rotaionSpeed * Vector3.up;
            //angularVelocity の速度で回転
            targetObject.transform.Rotate(angularVelocity * Time.deltaTime);
            displayMessage.text = $"Rotation Speed:{rotaionSign * rotaionSpeed:F1}[deg/s]";
        }

        void OnButtonClicked()
        {
            hasStarted = !hasStarted;
            targetObject.SetActive(hasStarted);
            //スライダーが操作可能か不可能か設定
            sliderForSpeed.interactable = hasStarted;
            //ドロップダウンが操作可能か不可能か設定
            dropdownForSpeedMode.interactable = hasStarted;
            //トグルが操作可能か不可能か設定
            toggleForReverse.interactable = hasStarted;
            //インプットフィールドが操作可能か不可能か設定
            inputFieldForSpeed.interactable = hasStarted;
            displayMessage.text = "";
        }

        //スライダーの値が変更されたときに、その値を rotaionSpeed に設定
        void OnSliderValueChanged(float value) => rotaionSpeed = value;
        //ロップダウンメニューの選択が変更されたときに、スライダーの最大値を更新
        void OnDropdownValueChanged(int index) => sliderForSpeed.maxValue = SpeedModeData[index].maxSpeed;
        //トグルボタンの状態が変更されたときに、回転の方向を更新
        void OnToggleValueChanged(bool isOn) => rotaionSign = isOn ? -1 : 1;
        // 入力フィールドが選択されたときに、その内容をクリア
        void OnInputFieldSelect(string text) => inputFieldForSpeed.text = "";
        //入力フィールドの編集が終了したときに、その値を rotaionSpeed に設定
        void OnInputFieldEndEdit(string text) => rotaionSpeed = float.TryParse(text, out var num) ? num:rotaionSpeed;

}

}
