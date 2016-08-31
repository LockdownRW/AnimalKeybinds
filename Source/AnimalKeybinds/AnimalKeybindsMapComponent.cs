using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace AnimalKeybinds
{
    public class AnimalKeybindsMapComponent : MapComponent
    {

        private KeyBindingDef previousAnimalKey = KeyBindingDef.Named("PreviousAnimal");
        private KeyBindingDef nextAnimalKey = KeyBindingDef.Named("NextAnimal");
        private MethodInfo remoteSingleSelectAndJumpTo;
        private List<object> remoteSelected;

        public override void MapComponentUpdate()
        {

            if (previousAnimalKey.KeyDownEvent)
            { 
                SelectPreviousAnimal();
            }
            else if (nextAnimalKey.KeyDownEvent)
            {
                SelectNextAnimal();
            }
            

        }

        public void getRemoteFieldsIfNeeded()
        {
            //retrieving private Core methods
            if (remoteSingleSelectAndJumpTo == null)
            {
                remoteSingleSelectAndJumpTo = typeof(Selector).GetMethod("SingleSelectAndJumpTo", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            //retrieving private Core fields
            if (remoteSelected == null)
            {
                remoteSelected = (List<object>) typeof(Selector).GetField("selected", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Find.Selector); //null because this is a static class. Would need an instance object of HealthCardUtility otherwise.
            }
        }

        public List<Pawn> getAnimalsList()
        {
            IEnumerable<Pawn> sorted = from p in Find.MapPawns.PawnsInFaction(Faction.OfPlayer)
                                       where p.RaceProps.Animal
                                       orderby p.RaceProps.petness descending, p.RaceProps.baseBodySize, p.def.label
                                       select p;
            return sorted.ToList();
        }

        public void SelectPreviousAnimal()
        {
            getRemoteFieldsIfNeeded();
            List<Pawn> list = getAnimalsList();


            if (list.Count == 0)
            {
                return;
            }
            int num = -1;
            for (int i = 0; i < this.remoteSelected.Count; i++)
            {
                Pawn pawn = this.remoteSelected[i] as Pawn;
                if (pawn != null)
                {
                    num = Mathf.Max(num, list.IndexOf(pawn));
                }
            }
            if (num == -1)
            {
                //this.SingleSelectAndJumpTo(list[list.Count - 1]);
                remoteSingleSelectAndJumpTo.Invoke(Find.Selector, new object[] { list[list.Count - 1] });
            }
            else
            {
                num--;
                if (num < 0)
                {
                    num += list.Count;
                }
                //this.SingleSelectAndJumpTo(list[num]);
                remoteSingleSelectAndJumpTo.Invoke(Find.Selector, new object[] { list[num] });
            }
        }

        public void SelectNextAnimal()
        {
            getRemoteFieldsIfNeeded();
            List<Pawn> list = getAnimalsList();

            if (list.Count == 0)
            {
                return;
            }
            int num = -1;
            for (int i = 0; i < this.remoteSelected.Count; i++)
            {
                Pawn pawn = this.remoteSelected[i] as Pawn;
                if (pawn != null)
                {
                    num = Mathf.Max(num, list.IndexOf(pawn));
                }
            }
            if (num == -1)
            {
                //Find.Selector.SingleSelectAndJumpTo(list[0]);
                remoteSingleSelectAndJumpTo.Invoke(Find.Selector, new object[] { list[0] });
            }
            else
            {
                //this.SingleSelectAndJumpTo(list[(num + 1) % list.Count]);
                remoteSingleSelectAndJumpTo.Invoke(Find.Selector, new object[] { list[(num + 1) % list.Count] });
            }
        }
    }
}
