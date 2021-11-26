using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBase.View.Poker
{
    public class Card : MonoBehaviour
    {
        public delegate void Callback();

        public int id;
        public Image image;
        public SpriteRenderer sprite;
        public AudioClip clickSound;
        public event Callback callback;
        public bool enableClick;

        public bool Clicked { get; private set; } = false;

        public Rect CardRect
        {
            get
            {
                if(image != null)
                {
                    return image.rectTransform.rect;
                }
                else
                {
                    return sprite.sprite.rect;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        public void SetCard(int card, Sprite img)
        {
            id = card;
            if (image != null)
            {
                image.sprite = img;
            }
            if (sprite != null)
            {
                sprite.sprite = img;
            }
        }

        public void OnClick()
        {
            if (enableClick)
            {
                Clicked = !Clicked;
                if (Clicked)
                {
                    if (image != null)
                    {
                        var rect = image.rectTransform;
                        rect.pivot = new Vector2(0.5f, 1);
                        rect.anchorMin = new Vector2(0.5f, 1);
                        rect.anchorMax = new Vector2(0.5f, 1);
                        rect.position = new Vector3(0, 0, 0);
                    }
                    if (sprite != null)
                    {
                        sprite.transform.position = new Vector3(0, 0.6f, 0);
                    }
                }
                else
                {
                    if (image != null)
                    {
                        var rect = image.rectTransform;
                        rect.pivot = new Vector2(0.5f, 0);
                        rect.anchorMin = new Vector2(0.5f, 0);
                        rect.anchorMax = new Vector2(0.5f, 0);
                        rect.position = new Vector3(0, 0, 0);
                    }
                    if (sprite != null)
                    {
                        sprite.transform.position = new Vector3(0, 0, 0);
                    }
                }
                callback();
                Present.GameMain.Instance.PlaySound(clickSound);
            }
        }
    }
}