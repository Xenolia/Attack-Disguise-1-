using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIJoystickController : MonoBehaviour
{
    [SerializeField] private InitParameters _initParamaters;
    bool stickEnabled = true;
    public void Init(InitParameters initParameters)
    {
        _initParamaters = initParameters;
    }
    public void DisableJoyStick()
    {
        stickEnabled = false;
        _initParamaters.JoystickImage.enabled = false;
        _initParamaters.JoystickImageBackground.enabled = false;
    }
    public void EnableJoyStick()
    {
        stickEnabled = true;
        
        _initParamaters.JoystickImage.enabled = true;
        _initParamaters.JoystickImageBackground.enabled = true;
    }

    private void Update()
    {
        
        if (Input.GetMouseButton(0)&&stickEnabled&&GameManager.Instance.controlEnabled)
        {
            // if (!EventSystem.current.IsPointerOverGameObject())
           //     return;
          //  EventSystem.current.RaycastAll(PointerEventData pointer)
 
            Vector2 pos = Input.mousePosition;
            if (!_initParamaters.JoystickImageBackground.enabled || !_initParamaters.JoystickImage.enabled)
            {


                _initParamaters.JoystickImage.enabled = true;
                _initParamaters.JoystickImageBackground.enabled = true;

                _initParamaters.JoystickImageBackground.rectTransform.position = pos;
                _initParamaters.JoystickImageBackground.rectTransform.localPosition = ClampPos(_initParamaters.JoystickImageBackground.rectTransform.position);
                _initParamaters.JoystickImage.rectTransform.position = _initParamaters.JoystickImageBackground.rectTransform.position;
            }
        }
        else
        {
            if (_initParamaters.JoystickImageBackground.enabled || _initParamaters.JoystickImage.enabled)
            {
                _initParamaters.JoystickImage.enabled = false;
                _initParamaters.JoystickImageBackground.enabled = false;
            }
        }


    }


    private Vector2 ClampPos(Vector2 pos)
    {
        pos = _initParamaters.JoystickImageBackground.rectTransform.localPosition;

        Vector3 minPosition = _initParamaters.Canvas.GetComponent<RectTransform>().rect.min - _initParamaters.JoystickImageBackground.rectTransform.rect.min;
        Vector3 maxPosition = _initParamaters.Canvas.GetComponent<RectTransform>().rect.max - _initParamaters.JoystickImageBackground.rectTransform.rect.max;

        pos.x = Mathf.Clamp(_initParamaters.JoystickImageBackground.rectTransform.localPosition.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(_initParamaters.JoystickImageBackground.rectTransform.localPosition.y, minPosition.y, maxPosition.y);

        return pos;
    }

    [System.Serializable]
    public class InitParameters
    {
        [SerializeField] private Image _joystickImage;
        [SerializeField] private Image _joystickImageBackground;
        [SerializeField] private Canvas _canvas;

        public Image JoystickImage => _joystickImage;
        public Image JoystickImageBackground => _joystickImageBackground;
        public Canvas Canvas => _canvas;

        public InitParameters(Image joystickImage, Image joystickImageBackground, Canvas canvas)
        {
            _joystickImage = joystickImage;
            _joystickImageBackground = joystickImageBackground;
            _canvas = canvas;
        }
    }
}
