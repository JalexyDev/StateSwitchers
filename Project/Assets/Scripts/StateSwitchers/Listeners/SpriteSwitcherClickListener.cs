using System.Collections.Generic;

public class SpriteSwitcherClickListener : BaseSpriteSwitcherListener
{
    public List<ImageState> imageStates;

    protected override List<IState> GetIStatesFromStates()
    {
        return new List<IState>(imageStates);
    }

    private void OnMouseDown()
    {
        stateSwitcher.Switch();
    }
}
