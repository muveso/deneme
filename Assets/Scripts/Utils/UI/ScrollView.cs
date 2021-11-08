using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Evade.Utils.UI {
    public static class ScrollView {
        const int NEW_ITEM_GAME_OBJECT_WIDTH = 800;
        const int NEW_ITEM_GAME_OBJECT_HEIGHT = 90;
        const int NEW_ITEM_GAME_OBJECT_FONT_SIZE = 30;

        public static GameObject CreateNewTextItemForScrollView(object item, int index) {
            GameObject newGameObject = new GameObject();
            Text myText = newGameObject.AddComponent<Text>();
            myText.text = $"{index}. {item}";
            myText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            myText.fontSize = NEW_ITEM_GAME_OBJECT_FONT_SIZE;
            myText.color = Color.black;
            newGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(NEW_ITEM_GAME_OBJECT_WIDTH,
                                                                                NEW_ITEM_GAME_OBJECT_HEIGHT);
            return newGameObject;
        }

    }
}