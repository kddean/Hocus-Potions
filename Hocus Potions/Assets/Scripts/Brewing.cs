using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Brewing : MonoBehaviour {


    class listItem {
       public int count;
       public Ingredient.Attributes attribute;
          
       public listItem(int c, Ingredient.Attributes a) {
            count = c;
            attribute = a;
        }
    }

    void Brew(Ingredient first, Ingredient second, Ingredient third) {
        List<listItem> attributes = new List<listItem>();
        Ingredient.Attributes[] attArray = new Ingredient.Attributes[] { first.attributeList[0], first.attributeList[1], first.attributeList[2],
                                                                        second.attributeList[0], second.attributeList[1], second.attributeList[2],
                                                                        third.attributeList[0], third.attributeList[1], third.attributeList[2]};

        Ingredient.Attributes primary;
        Ingredient.Attributes? secondary = null;
        Ingredient.Modifiers? mod = null;
        int duration = 0;
        Sprite image;
        string name = "";

        for(int i = 0; i < 9; i++) {
            foreach (listItem item in attributes) {
                if (attArray[i] == item.attribute) {
                    item.count++;
                    break;
                } else {
                    attributes.Add(new listItem(1, attArray[i]));
                }
            }
        }

        List<listItem> sorted = attributes.OrderBy(listItem => listItem.count).ToList();

        //Find the first item in the list that isnt a modifier
        int index = 0;
        while (index < sorted.Count() && System.Enum.IsDefined(typeof(Ingredient.Modifiers), sorted[index].attribute)) {
            index++;
        }

        //If all the items are modifiers then it fails
        if (index >= sorted.Count()) {
            Debug.Log("potion creation failed");
        } else {
            if (sorted[index].count >= 3) {
                //accounts for ties in primary attribute
                if (sorted[index].count == sorted[index + 1].count) {
                    int flip = Random.Range(0, 2);
                    if (flip == 0) {
                        primary = sorted[index].attribute;
                    } else {
                        primary = sorted[index+1].attribute;
                    }
                    //if it's not a tie just use the first attribute
                } else {
                    primary = sorted[index].attribute;
                }

                if (index == sorted.Count() - 2) {
                    secondary = null;
                } else {
                    if (sorted[index + 1].count >= 2) {
                        //accounts for ties in secondary attribute
                        if (sorted[index + 1].count == sorted[index + 2].count) {
                            int flip = Random.Range(0, 2);
                            if (flip == 0) {
                                secondary = sorted[index + 1].attribute;
                            } else {
                                secondary = sorted[index + 2].attribute;
                            }
                            //if it's not a tie just use the second attribute
                        } else {
                            secondary = sorted[index + 1].attribute;
                        }
                    } else {
                        secondary = null;
                    }
                }

                //Modifiers
                index = 0;
                //find the first modifier in the list
                while (index < sorted.Count() && !System.Enum.IsDefined(typeof(Ingredient.Modifiers), sorted[index].attribute)) {
                    index++;
                }
             
                if(index >= sorted.Count) {
                    mod = null;
                } else {
                    mod = (Ingredient.Modifiers) System.Enum.Parse(typeof(Ingredient.Modifiers), sorted[index].attribute.ToString());
                }

                //need to set durations, sprite, and name based on the type of potion before it's created 
                //create the potion; this needs more added to it to add the potion to your inventory, display ui, etc. 
                Potion pot = new Potion(name, image, duration, primary, secondary, mod);
            } else {
                Debug.Log("Potion creation failed");
            }

        }
        
  

    }
}
