using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlimUI.ModernMenu{
	[System.Serializable]
	public class ThemedUIElement : ThemedUI {
		[Header("Parameters")]
		Color outline;
		Image image;
		GameObject message;
		public enum OutlineStyle {solidThin, solidThick, dottedThin, dottedThick};
		public bool hasImage = false;
		public bool isText = false;

		protected override void OnSkinUI(){
			base.OnSkinUI();

			if(hasImage){
				image = GetComponent<Image>();
				image.color = themeController.currentColor;
			}

			message = gameObject;

			if(isText){
				TextMeshProUGUI textMeshProUGUI = message.GetComponent<TextMeshProUGUI>();
				if (textMeshProUGUI != null)
				{
					textMeshProUGUI.color = themeController.textColor;
				}

				TextMeshPro textMeshPro = message.GetComponent<TextMeshPro>();
				if (textMeshPro != null)
				{
					textMeshPro.color = themeController.textColor;
				}
			}
		}
	}
}