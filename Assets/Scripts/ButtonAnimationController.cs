using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAnimationController : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventdata)
    {
        Debug.Log(gameObject + " clicked");
        GetComponent<Animator>().Play("anim");
    }

}