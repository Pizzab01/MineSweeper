using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.Lifecycle;
using AndroidX.ViewPager2.Adapter;
using MineSweeper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Content.ClipData;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace MineSweeper
{
    public class FragmentPageAdapter : FragmentStateAdapter
    {
        //AndroidX.Fragment.App.Fragment[] fragments;
        private Fragment fragment = new AndroidX.Fragment.App.Fragment();

        private readonly int itemCount;
        public FragmentPageAdapter(FragmentManager fragmentManager, Lifecycle lifecylce, int itemCount) : base(fragmentManager, lifecylce)
        {
            this.itemCount = itemCount;
        }

        public override int ItemCount => itemCount; // fragments.Length;

        public override Fragment CreateFragment(int position)
        {
            //return fragments[position];

            if (position == 0)
            {
                fragment = new BeginnerFragment();
            }
            else if (position == 1)
            {
                fragment = new IntermediateFragment();
            }
            else
            {
                fragment = new ExpertFragment();
            }
            return fragment;
        }
    }
}