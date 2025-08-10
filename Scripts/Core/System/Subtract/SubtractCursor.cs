using UnityEngine;
using UnityEngine.UI;
using MantenseiLib;

namespace GameJam_HIKU
{
    /// <summary>UI Canvasを使用したカーソル表示システム</summary>
    public class SubtractCursor : MonoBehaviour
    {
        [field: SerializeField] public GameObject NormalCursorPrefab { get; private set; }
        [field: SerializeField] public GameObject TargetCursorPrefab { get; private set; }

        [field: SerializeField] public Canvas TargetCanvas { get; private set; }
        [field: SerializeField] public bool CreateOwnCanvas { get; private set; } = true;

        [field: SerializeField] public bool HideSystemCursor { get; private set; } = true;
        [field: SerializeField] public Vector2 CursorOffset { get; private set; } = Vector2.zero;

        [field: SerializeField] public Image CooldownFillImage { get; private set; }
        [field: SerializeField] public Color CooldownColor { get; private set; } = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        [field: SerializeField] public Color ReadyColor { get; private set; } = new Color(1f, 1f, 1f, 1f);

        // SubtractSkillの参照を追加
        [GetComponent(HierarchyRelation.Self | HierarchyRelation.Parent)]
        SubtractSkill subtractSkill;

        private GameObject currentCursorInstance;
        private GameObject normalCursorInstance;
        private GameObject targetCursorInstance;
        private bool isTargetMode = false;

        void Start()
        {
            if (HideSystemCursor)
            {
                Cursor.visible = false;
            }

            SetupCanvas();
            CreateCursorInstances();
            SetupCooldownDisplay();
            SetNormalCursor();
        }

        /// <summary>Canvas設定</summary>
        private void SetupCanvas()
        {
            if (CreateOwnCanvas && TargetCanvas == null)
            {
                GameObject canvasObj = new GameObject("CursorCanvas");

                TargetCanvas = canvasObj.AddComponent<Canvas>();
                TargetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                TargetCanvas.sortingOrder = 999;

                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>().enabled = false;
            }
        }

        /// <summary>カーソルインスタンス作成</summary>
        private void CreateCursorInstances()
        {
            if (NormalCursorPrefab != null)
            {
                normalCursorInstance = Instantiate(NormalCursorPrefab, TargetCanvas.transform);
                normalCursorInstance.SetActive(false);
                SetupCursorRectTransform(normalCursorInstance);
                NormalCursorPrefab.SetActive(false);
            }

            if (TargetCursorPrefab != null)
            {
                targetCursorInstance = Instantiate(TargetCursorPrefab, TargetCanvas.transform);
                targetCursorInstance.SetActive(false);
                SetupCursorRectTransform(targetCursorInstance);
                TargetCursorPrefab.SetActive(false);
            }
        }

        /// <summary>クールダウン表示の設定</summary>
        private void SetupCooldownDisplay()
        {
            // NormalCursorPrefab内からFill用のImageを探す
            if (CooldownFillImage == null && normalCursorInstance != null)
            {
                // "Fill"という名前の子オブジェクトを探す
                Transform fillTransform = normalCursorInstance.transform.Find("Fill");
                if (fillTransform == null)
                {
                    // または最初に見つかったImageコンポーネントを使用
                    CooldownFillImage = normalCursorInstance.GetComponentInChildren<Image>();
                }
                else
                {
                    CooldownFillImage = fillTransform.GetComponent<Image>();
                }
            }

            // FillAmountが使えるように設定
            if (CooldownFillImage != null)
            {
                CooldownFillImage.type = Image.Type.Filled;
                CooldownFillImage.fillMethod = Image.FillMethod.Radial360;
                CooldownFillImage.fillOrigin = (int)Image.Origin360.Top;
                CooldownFillImage.fillClockwise = true;
            }
        }

        /// <summary>カーソルのRectTransform設定</summary>
        private void SetupCursorRectTransform(GameObject cursorObj)
        {
            var rectTransform = cursorObj.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = cursorObj.AddComponent<RectTransform>();
            }

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        void Update()
        {
            UpdateCursorPosition();
            UpdateCooldownDisplay();
        }

        /// <summary>クールダウン表示の更新</summary>
        private void UpdateCooldownDisplay()
        {
            if (CooldownFillImage == null || subtractSkill == null) return;

            // スキル使用可能状態のチェック
            if (subtractSkill.IsSkillActive)
            {
                // スキル使用中は非表示または特別な表示
                CooldownFillImage.fillAmount = 1f;
                CooldownFillImage.color = ReadyColor;
            }
            else if (subtractSkill.IsOnCooldown)
            {
                // クールダウン中
                float cooldownProgress = subtractSkill.CooldownProgress;
                CooldownFillImage.fillAmount = cooldownProgress;
                CooldownFillImage.color = Color.Lerp(CooldownColor, ReadyColor, cooldownProgress);
            }
            else
            {
                // 使用可能
                CooldownFillImage.fillAmount = 1f;
                CooldownFillImage.color = ReadyColor;
            }
        }

        /// <summary>カーソル位置を更新</summary>
        private void UpdateCursorPosition()
        {
            if (currentCursorInstance == null) return;

            var rectTransform = currentCursorInstance.GetComponent<RectTransform>();
            if (rectTransform == null) return;

            Vector2 mousePos = Input.mousePosition;
            mousePos += CursorOffset;

            rectTransform.anchoredPosition = mousePos;
        }

        /// <summary>通常カーソルに切り替え</summary>
        public void SetNormalCursor()
        {
            if (normalCursorInstance == null) return;

            if (currentCursorInstance != null && currentCursorInstance != normalCursorInstance)
            {
                currentCursorInstance.SetActive(false);
            }

            normalCursorInstance.SetActive(true);
            currentCursorInstance = normalCursorInstance;
            isTargetMode = false;
        }

        /// <summary>ターゲットカーソルに切り替え</summary>
        public void SetTargetCursor()
        {
            if (targetCursorInstance == null) return;

            if (currentCursorInstance != null && currentCursorInstance != targetCursorInstance)
            {
                currentCursorInstance.SetActive(false);
            }

            targetCursorInstance.SetActive(true);
            currentCursorInstance = targetCursorInstance;
            isTargetMode = true;
        }

        public void HideCursor()
        {
            if (currentCursorInstance != null)
            {
                currentCursorInstance.SetActive(false);
            }
        }

        public void ShowCursor()
        {
            if (currentCursorInstance != null)
            {
                currentCursorInstance.SetActive(true);
            }
        }

        public bool IsTargetMode() => isTargetMode;

        void OnDestroy()
        {
            if (HideSystemCursor)
            {
                Cursor.visible = true;
            }

            if (normalCursorInstance != null)
                Destroy(normalCursorInstance);
            if (targetCursorInstance != null)
                Destroy(targetCursorInstance);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (HideSystemCursor)
            {
                Cursor.visible = !hasFocus;
            }
        }
    }
}