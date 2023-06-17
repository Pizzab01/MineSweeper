using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Google.Android.Material.Tabs.TabLayoutMediator;



namespace MineSweeper
{
    [Activity(Label = "HighScoreFragmentActivity")]
    public class HighScoreFragmentActivity : FragmentActivity
    {

        ITabConfigurationStrategy ss;
        private const String TAG = "TabFragment Activity";
        private ViewPager2 viewPager;
        private FragmentPageAdapter pagerAdapter;
        private TabLayout tabLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.fragment_layout);

            // setup
            viewPager = FindViewById<ViewPager2>(Resource.Id.viewPager);
            tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
        
            pagerAdapter = new FragmentPageAdapter(this.SupportFragmentManager, this.Lifecycle, 3);

            // Set the adapter onto the view pager
            viewPager.Adapter = pagerAdapter;
            viewPager.Adapter.NotifyDataSetChanged();

            new TabLayoutMediator(tabLayout, viewPager, new TabConfigurationStrategy(ApplicationContext)).Attach();
        }


        class TabConfigurationStrategy : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
        {
            // setup tab cofiguration
            private readonly Context context;

            public TabConfigurationStrategy(Context context)
            {
                this.context = context;
            }

            public void OnConfigureTab(TabLayout.Tab tab, int position) // set tab names based on position
            {
                tab.SetText(context.Resources.GetStringArray(Resource.Array.tabs)[position]);
            }
        }


        
    }
}