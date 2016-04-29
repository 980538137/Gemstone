using UnityEngine;
using System.Collections;

public class Gemstone : MonoBehaviour {
    public float xOffset = -5.5f;
    public float yOffset = -2.5f;
    public int rowIndex = 0;
    public int columIndex = 0;

    public GameObject[] gemstoneBgs;//宝石的数组
    public int gemstoneType;//宝石的类型
    public GameController gameController;

    public SpriteRenderer spriteRenderer;
    public bool isSelected
    {
        set
        {
            if(value)
            {
                spriteRenderer.color = Color.red;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }

    private GameObject gemstoneBg;

    void Awake()
    {
        
    }

	// Use this for initialization
	void Start () {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        spriteRenderer = gemstoneBg.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdatePosition(int _rowIndex,int _columIndex)
    {
        rowIndex = _rowIndex;
        columIndex = _columIndex;
        this.transform.position = new Vector3(xOffset + columIndex, yOffset + rowIndex, 0);
    }

    public void TweenToPosition(int _rowIndex,int _columIndex)
    {
        rowIndex = _rowIndex;
        columIndex = _columIndex;
        iTween.MoveTo(this.gameObject, iTween.Hash("x", xOffset + columIndex, "y", yOffset + rowIndex, "time", 0.5f));
    }

    public void RandomCreateGemstoneBg()//生成随机的宝石类型
    {
        if(gemstoneBg != null)
        {
            return;
        }
        gemstoneType = Random.Range(0, gemstoneBgs.Length);
        gemstoneBg = Instantiate(gemstoneBgs[gemstoneType]) as GameObject;
        gemstoneBg.transform.parent = this.transform;
    }

    public void OnMouseDown()
    {
        gameController.Select(this);
    }

    public void Dispose()
    {
        Destroy(this.gameObject);
        Destroy(gemstoneBg);
        gameController = null;
    }
}
