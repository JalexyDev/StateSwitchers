using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class BaseSpriteSwitcherListener : BaseSwitcherListener
{
    public int afterFinishStateIndex;

    protected SpriteRenderer spriteRenderer;

    protected override void OnInit()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void OnStart()
    {
        stateSwitcher.StartSwitching();
    }

    protected override Action<IState> GetBetweenStateAction()
    {
        return (state) =>
        {
            SetSprite((state as ImageState).Sprite);
        };
    }

    protected void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    protected override Action<IState> GetLastStateAction()
    {
        return (state) =>
        {
            SetSprite((state as ImageState).Sprite);
        };
    }

    protected override Action<IState> GetFinishStateAction()
    {
        return (ImageState) =>
        {
            SelectState(afterFinishStateIndex);
        };
    }
}
