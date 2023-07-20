using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace PirexGames.UI.Base
{
    public class ButtonHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const float PREVENT_MULTIPLE_BUTTON_CLICK_COOLDOWN = 0.3f;

        [SerializeField] Selectable selectableController;

        [Header("Scale Effect"), Space(16)]
        [SerializeField] private bool isPlayScaleEffect = false;
        [SerializeField] private Vector3 scaleEnd = new Vector3(0.9f, 0.9f, 0.9f);
        [SerializeField] private float duration = 0.2f;

        [Header("Prevent spam"), Space(16)]
        [SerializeField] private bool isPreventSpamClick = true;
        [SerializeField] private int spamCooldownInMs = 400;


        [Header("NEED TO SPLIT THESE CODES"), Space(16)]
        [SerializeField] private Sprite touchColorEffect;
        [SerializeField] private Image jumpImg;


        [SerializeField] private bool useCustomTransform = false;
        [SerializeField] private Transform customTransform;

        private static float s_lastTimeStampTriggerAction;

        private Tweener scaleTween = default;

        private Vector3 defaultSize;

        private byte pointerDownCurrentlyActiveCount = 0;

        private Sprite normalColorEffect;

#if UNITY_EDITOR
        [ContextMenu("Get Selectable Ref")]
        public void Reset()
        {
            selectableController = gameObject.GetComponent<Selectable>();

            if (!selectableController)
            {
                selectableController = gameObject.AddComponent<Selectable>();
            }

            Navigation navigation = selectableController.navigation;
            navigation.mode = Navigation.Mode.None;
            selectableController.navigation = navigation;

            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif

        private void Awake()
        {
            try
            {
                defaultSize = useCustomTransform ? customTransform.localScale : transform.localScale;
                Navigation navigation = selectableController.navigation;
                navigation.mode = Navigation.Mode.None;
                selectableController.navigation = navigation;

                if (jumpImg)
                    normalColorEffect = jumpImg.sprite;
            }
            catch
            {
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //prevent duplicate click on button: only consider first pointer down as click action input----------------------------------------------

            pointerDownCurrentlyActiveCount++;

            if (pointerDownCurrentlyActiveCount > 1)
            {
                return;
            }
            //----------------------------------------------------------------------------------------------------------------------------------------


            if (!selectableController.interactable || !selectableController.enabled)
            {
                return;
            }

            if (isPlayScaleEffect)
            {
                if (scaleTween != default)
                    scaleTween.Complete();

                defaultSize = useCustomTransform ? customTransform.localScale : transform.localScale;

                Vector3 targetSize = new Vector3(defaultSize.x * scaleEnd.x, defaultSize.y * scaleEnd.y, defaultSize.z * scaleEnd.z);

                scaleTween = useCustomTransform ?
                customTransform.DOScale(targetSize, duration) :
                transform.DOScale(targetSize, duration);

                if (jumpImg)
                    jumpImg.sprite = touchColorEffect;
            }

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //prevent duplicate click on button: only consider last pointer up as click action input-------------------------------------------------
            pointerDownCurrentlyActiveCount--;

            if (pointerDownCurrentlyActiveCount > 0)
            {
                return;
            }
            //----------------------------------------------------------------------------------------------------------------------------------------

            if (!selectableController.interactable || !selectableController.enabled)
            {
                return;
            }


            if (isPlayScaleEffect)
            {
                if (scaleTween != default)
                    scaleTween.Complete();

                if (useCustomTransform)
                    scaleTween = customTransform.DOScale(defaultSize, duration);
                else
                    scaleTween = transform.DOScale(defaultSize, duration);

                if (jumpImg)
                    jumpImg.sprite = normalColorEffect;
            }

            if (isPreventSpamClick)
            {
                //prevent spam click on a button: temporarily disable the raycast for the next interval, prevent the user click continously------------------------------

                PreventSpamCoolDownMultipleClick();

                //-------------------------------------------------------------------------------------------------------------------------------------------------------

                //prevent spam click on multiple buttons: temporarily disable the button, so the button click action don't trigger in the current frame------------------

                float curTime = Time.time;
                if (curTime <= s_lastTimeStampTriggerAction + PREVENT_MULTIPLE_BUTTON_CLICK_COOLDOWN)
                {
                    PreventSpamCoolDownMultipleButtonsClick();
                }
                else
                {
                    s_lastTimeStampTriggerAction = curTime;
                }

                //-------------------------------------------------------------------------------------------------------------------------------------------------------

            }
        }

        private async void PreventSpamCoolDownMultipleClick()
        {
            selectableController.image.raycastTarget = false;

            await UniTask.Delay(spamCooldownInMs);

            if (selectableController || selectableController.image)
            {
                selectableController.image.raycastTarget = true;
            }
        }

        private async void PreventSpamCoolDownMultipleButtonsClick()
        {
            selectableController.interactable = false;

            await UniTask.DelayFrame(1);

            if (selectableController)
            {
                selectableController.interactable = true;
            }
        }

    }
}