using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
    public Gemstone gemstone;
    public int rowNum = 7;//行数
    public int columNum = 10;//列数
    public ArrayList gemstoneList;
    public AudioClip match3Clip;
    public AudioClip swapClip;
    public AudioClip errorClip;

    AudioSource audio;

    private ArrayList matchesGemstone;
    private Gemstone currentGemstone;
    

	// Use this for initialization
	void Start () {
        gemstoneList = new ArrayList();
        matchesGemstone = new ArrayList();
        audio = GetComponent<AudioSource>();

        for(int rowIndex = 0;rowIndex < rowNum;rowIndex++)
        {
            ArrayList temp = new ArrayList();
            for(int columIndex = 0;columIndex < columNum;columIndex++)
            {
                Gemstone c = AddGemstone(rowIndex, columIndex);
                temp.Add(c);
                
                
            }
            gemstoneList.Add(temp);
        }

        if (CheckHoriozntalMatches() || CheckVerticalMatches())
        {
            RemoveMatches();
        }
        
	}

    public Gemstone AddGemstone(int rowIndex,int columIndex)
    {
        Gemstone c = Instantiate(gemstone) as Gemstone;
        c.transform.parent = this.transform;
        c.GetComponent<Gemstone>().RandomCreateGemstoneBg();
        c.GetComponent<Gemstone>().UpdatePosition(rowIndex, columIndex);
        //c.GetComponent<Gemstone>().TweenToPosition(rowIndex, columIndex);
        return c;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Select(Gemstone c)
    {
        //Destroy(c.gameObject);
        if(currentGemstone == null)
        {
            currentGemstone = c;
            currentGemstone.isSelected = true;
            return;
        }
        else
        {
            if((Mathf.Abs(currentGemstone.rowIndex - c.rowIndex) + Mathf.Abs(currentGemstone.columIndex - c.columIndex)) == 1)
            {
                StartCoroutine(ExchangeAndMatches(currentGemstone, c));
            }
            else
            {
                audio.PlayOneShot(errorClip);
            }
           

            currentGemstone.isSelected = false;
            currentGemstone = null;
        }
    }

    IEnumerator ExchangeAndMatches(Gemstone c1,Gemstone c2)
    {
        Exchange(c1, c2);
        yield return new WaitForSeconds(0.5f);
        if(CheckHoriozntalMatches() || CheckVerticalMatches())
        {
            RemoveMatches();
        }
        else
        {
            Exchange(c2, c1);
        }
    }
    
    //检测水平方向
    bool CheckHoriozntalMatches()
    {
        bool isMatches = false;
        for (int rowIndex = 0; rowIndex < rowNum;rowIndex++ )
        {
            for(int columIndex = 0;columIndex < columNum - 2;columIndex++)
            {
                if(GetGemstone(rowIndex,columIndex).gemstoneType == GetGemstone(rowIndex,columIndex + 1).gemstoneType &&GetGemstone(rowIndex,columIndex).gemstoneType == GetGemstone(rowIndex,columIndex + 2).gemstoneType)
                {
                    Debug.Log("发现行相同的宝石");
                    AddMatches(GetGemstone(rowIndex, columIndex));
                    AddMatches(GetGemstone(rowIndex, columIndex + 1));
                    AddMatches(GetGemstone(rowIndex, columIndex + 2));
                    isMatches = true;
                }
            }
        }
        return isMatches;
    }

    //检测垂直方向
    bool CheckVerticalMatches()
    {
        bool isMatches = false;
        for (int columIndex = 0; columIndex < columNum; columIndex++)
        {
            for (int rowIndex = 0; rowIndex < rowNum - 2; rowIndex++)
            {
                if (GetGemstone(rowIndex, columIndex).gemstoneType == GetGemstone(rowIndex + 1, columIndex).gemstoneType && GetGemstone(rowIndex, columIndex).gemstoneType == GetGemstone(rowIndex + 2, columIndex).gemstoneType)
                {
                    Debug.Log("发现列相同的宝石");
                    AddMatches(GetGemstone(rowIndex, columIndex));
                    AddMatches(GetGemstone(rowIndex + 1, columIndex));
                    AddMatches(GetGemstone(rowIndex + 2, columIndex));
                    isMatches = true;
                }
            }
        }
        return isMatches;
    }

    void AddMatches(Gemstone c)
    {
        if(matchesGemstone == null)
        {
            matchesGemstone = new ArrayList();
        }
        int index = matchesGemstone.IndexOf(c);
        if(index == -1)
        {
            matchesGemstone.Add(c);
        }
    }

    //删除匹配的宝石
    void RemoveMatches()
    {
        for(int i = 0;i < matchesGemstone.Count;i++)
        {
            Gemstone c = matchesGemstone[i] as Gemstone;
            RemoveGemstone(c);
        }
        matchesGemstone = new ArrayList();
        StartCoroutine(WaitForCheckMatchesAgain());
    }

    IEnumerator WaitForCheckMatchesAgain()
    {
        yield return new WaitForSeconds(0.5f);
        if(CheckHoriozntalMatches() || CheckVerticalMatches())
        {
            RemoveMatches();
        }
    }
    void RemoveGemstone(Gemstone c)
    {
        c.Dispose();
        audio.PlayOneShot(match3Clip);
        for(int i = c.rowIndex + 1;i < rowNum;i++)
        {
            Gemstone tempGemstone = GetGemstone(i,c.columIndex);
            tempGemstone.rowIndex--;
            SetGemstone(tempGemstone.rowIndex, tempGemstone.columIndex, tempGemstone);
            //tempGemstone.UpdatePosition(tempGemstone.rowIndex, tempGemstone.columIndex);
            tempGemstone.TweenToPosition(tempGemstone.rowIndex, tempGemstone.columIndex);
        }

        Gemstone newGemstone = AddGemstone(rowNum,c.columIndex);
        newGemstone.rowIndex--;
        SetGemstone(newGemstone.rowIndex, newGemstone.columIndex, newGemstone);
        //newGemstone.UpdatePosition(newGemstone.rowIndex, newGemstone.columIndex);
        newGemstone.TweenToPosition(newGemstone.rowIndex, newGemstone.columIndex);
    }

    //通过行号和列号取得对应位置的宝石
    public Gemstone GetGemstone(int rowIndex,int columIndex)
    {
        ArrayList temp = gemstoneList[rowIndex] as ArrayList;
        Gemstone c = temp[columIndex] as Gemstone;
        return c;
    }

    public void SetGemstone(int rowIndex,int columIndex,Gemstone c)
    {
        ArrayList temp = gemstoneList[rowIndex] as ArrayList;
        temp[columIndex] = c;
    }

    //实现宝石之间交换位置
    public void Exchange(Gemstone c1,Gemstone c2)
    {
        audio.PlayOneShot(swapClip);
        SetGemstone(c1.rowIndex, c1.columIndex, c2);
        SetGemstone(c2.rowIndex, c2.columIndex, c1);

        //交换c1和c2的行号
        int tempRowIndex = c1.rowIndex;
        c1.rowIndex = c2.rowIndex;
        c2.rowIndex = tempRowIndex;

        //交换c1和c2的列号
        int tempColumIndex = c1.columIndex;
        c1.columIndex = c2.columIndex;
        c2.columIndex = tempColumIndex;

        //c1.UpdatePosition(c1.rowIndex, c1.columIndex);
        //c2.UpdatePosition(c2.rowIndex, c2.columIndex);

        c1.TweenToPosition(c1.rowIndex, c1.columIndex);
        c2.TweenToPosition(c2.rowIndex, c2.columIndex);
    }


}
