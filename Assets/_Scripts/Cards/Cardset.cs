using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

public class Cardset : MonoBehaviour
{
    public List<ItemSet> itemSets;
    public List<CardBackgroundSet> cardBacks;

    public string fileName;
    //private XDocument xDoc;
    //private IEnumerable<XElement> items;
    //private List<CardXMLData> data = new List<CardXMLData>();

    private string currentImageSet;

    private void Awake()
    {
        //xDoc = XDocument.Load("Assets/Resources/Documents/" + fileName + ".xml");
        //items = xDoc.Descendants("chapter_1").Elements();
        //itemSets = new List<ItemSet>();
        //StartCoroutine("AssignData");
    }

    //private IEnumerator AssignData()
    //{
    //    foreach (var item in items)
    //    {
    //        ItemSet newSet = new ItemSet();
    //        newSet.setName = item.Attribute("name").Value;
    //        newSet.items = new List<CardData>();
    //        IEnumerable<XElement> subItems = item.Parent.Descendants("imageset").Elements();
    //        foreach (var subItem in subItems)
    //        {
    //            if (subItem.Parent.Attribute("name").Value != item.Attribute("name").Value) continue;
    //            Material mat = Resources.Load("Materials/Items/" + subItem.Parent.Attribute("folderName").Value + "/" + subItem.Element("material").Value.Trim().ToString()) as Material;
    //            if (mat == null)
    //            {
    //                Texture2D tex = Resources.Load("Textures/Items/" + subItem.Parent.Attribute("folderName").Value + "/" + subItem.Element("texture").Value.Trim().ToString()) as Texture2D;
    //                //if (!tex.alphaIsTransparency) tex.alphaIsTransparency = true;
    //                mat = new Material(Shader.Find("Standard"));
    //                mat.mainTexture = tex;
    //                mat.SetFloat("_Mode", 1);
    //                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
    //                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
    //                mat.SetInt("_ZWrite", 1);
    //                mat.EnableKeyword("_ALPHATEST_ON");
    //                mat.DisableKeyword("_ALPHABLEND_ON");
    //                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    //                mat.renderQueue = 2450;
    //                mat.SetInt("_Cutoff", 1);
    //            }

    //            Vector2 newVec = new Vector2(float.Parse(subItem.Element("scalex").Value), float.Parse(subItem.Element("scaley").Value));
    //            newSet.items.Add(new CardData(0, mat, newVec));
    //        }

    //        itemSets.Add(newSet);
    //    }
    //    yield return null;
    //}

    public List<CardData> GetItems(List<string> sets)
    {
        int idNum = 0;
        List<CardData> data = new List<CardData>();
        for (int i = 0; i < sets.Count; ++i)
        {
            for (int j = 0; j < itemSets.Count; ++j)
            {
                if (itemSets[j].setName == sets[i])
                {
                    for (int k = 0; k < itemSets[j].items.Count; ++k)
                    {
                        CardData temp = itemSets[j].items[k];
                        CardData cd = new CardData(idNum, temp.cardMat, temp.cardImageScale);
                        idNum++;
                        data.Add(cd);
                    }
                    //data.AddRange(itemSets[j].items);
                }
            }    
        }

        return data;
    }

    public List<Material> GetBacks(List<string> sets)
    {
        List<Material> data = new List<Material>();

        for (int i = 0; i < sets.Count; ++i)
        {
            for (int j = 0; j < cardBacks.Count; ++j)
            {
                if (cardBacks[j].name == sets[i])
                {
                    for (int k = 0; k < cardBacks[j].images.Count; ++k)
                    {
                        data.Add(cardBacks[j].images[k]);
                    }
                }
            }
        }

        return data;
    }
}

[System.Serializable]
public class ItemSet
{
    public string setName;
    public List<CardData> items;
}

[System.Serializable]
public class CardBackgroundSet
{
    public string name;
    public List<Material> images;
}

public class CardXMLData
{
    public int pageNum;
    public string charText, dialogueText;
    public CardXMLData(int page, string character, string dialogue)
    {
        pageNum = page;
        charText = character;
        dialogueText = dialogue;
    }
}