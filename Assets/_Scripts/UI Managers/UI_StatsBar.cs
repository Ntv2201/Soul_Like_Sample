using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_StatsBar : MonoBehaviour
{
    private Slider slider;
    // variable to scale bar size depending on stat (highger = longer bar accross screen)
    // secondary bar behind may bar for polish effect (for ex: yellow bar that shows how much action/damage takes away from current stats)

    protected virtual void Awake()
    {
        slider = GetComponent<Slider>(); 
    }

    public virtual void SetStat(int newValue)
    {
        slider.value = newValue;
    }
    
    public virtual void SetMaxStat(int maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;
    }

}
