using UnityEngine;

public class TileScript : MonoBehaviour
{

    public GameObject highlight;
    private GameObject _tileHighlight;
    private TileManager _tileManager;
    public int TilePlacement{ get; private set; }


    public void HighlightTile()
    {
        _tileHighlight.SetActive(true);
    }

    public void UnHighlightTile()
    {
        _tileHighlight.SetActive(false);
    }
    void Start()
    {
        _tileManager = gameObject.GetComponentInParent<TileManager>();
        _tileHighlight = transform.Find("TileHighlight").gameObject;
        Vector3 localPosition = transform.localPosition;
        Vector3 localScale = transform.localScale;
        TilePlacement = (int)localPosition.z/(int)(10*localScale.z) * 8 + (int)localPosition.x/(int)(10*localScale.x);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _tileManager.clickTile(TilePlacement);
        }
    }

    private void OnMouseEnter()
    {
        if (!BoardManager._humainPlayer) return;
        highlight.SetActive(true);
        highlight.transform.position = transform.position;
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }
}
