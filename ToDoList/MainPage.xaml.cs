using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ToDoList
{
    public partial class MainPage : ContentPage
    {
        private Queue<FlexLayout> missions = new Queue<FlexLayout>();
        private Stack<Mission> undos = new Stack<Mission>();

        public MainPage()
        {
            InitializeComponent();
            DeleteMissions();
        }

        private void ImageButton_OnPressed(object sender, EventArgs e)
        {
            try
            {
                if (TaskText.Text.Trim() == "")
                {
                    TaskText.Text = "";
                    return;
                }
            }
            catch (SystemException exception)
            {
                Console.WriteLine(TaskText.Text);
                TaskText.Text = "";
                return;
            }

            FlexLayout triada = new FlexLayout
            {
                JustifyContent = FlexJustify.SpaceBetween,
                BackgroundColor = new Color(0, 0, 0, 0),
                AlignItems = FlexAlignItems.Center,
                ScaleY = 0
            };

            CheckBox completeMission = new CheckBox();
            completeMission.CheckedChanged += CheckBox_OnCheckedChanged;

            ImageButton trash = new ImageButton
            {
                WidthRequest = 40,
                HeightRequest = 40,
                BackgroundColor = new Color(0, 0, 0, 0),
                Source = "https://cdn1.iconfinder.com/data/icons/objects-16/512/105552_-_bin_can_garbage_trash-256.png"

            };
            trash.Clicked += Trash_OnPressed;

            Label mission = new Label
            {
                Text = $"{TaskText.Text.Trim()}",
                TextColor = new Color(0, 0, 0),
                BackgroundColor = new Color(0, 0, 0, 0),
                WidthRequest = 200,
                FontAttributes = FontAttributes.None,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            triada.Children.Add(completeMission);
            triada.Children.Add(mission);
            triada.Children.Add(trash);

            CreateChildren(triada);
            TaskText.Text = "";
        }

        private void CreateChildren(FlexLayout childMission)
        {
            Tasks.Children.Add(childMission);
            childMission.ScaleYTo(1, 300, Easing.SinOut);
        }

        private void CreateChildren(Mission mission)
        {
            Tasks.Children.Insert(mission.index, mission.mission);
            mission.mission.ScaleYTo(1, 300, Easing.SinOut);
        }

        private void Trash_OnPressed(object sender, EventArgs e)
        {
            missions.Enqueue(((sender as ImageButton).Parent) as FlexLayout);
        }

        private void CheckBox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            ((((sender as CheckBox).Parent) as FlexLayout).Children[1] as Label).FontAttributes =
                e.Value ? FontAttributes.Bold : FontAttributes.None;
        }

        private async void DeleteMissions()
        {
            if (missions.Count != 0)
            {
                missions.Peek().ScaleYTo(0, 250, Easing.SinIn);
                HelpDeleteMission(missions.Dequeue());
            }

            await Task.Delay(20);
            DeleteMissions();
        }

        private async void HelpDeleteMission(FlexLayout parent)
        {
            await Task.Delay(250);
            undos.Push(new Mission(Tasks.Children.IndexOf(parent), parent));
            Tasks.Children.Remove(parent);
        }

        private void Undo_OnPressed(object sender, EventArgs e) { if (undos.Count != 0) CreateChildren(undos.Pop()); }

        public struct Mission
        {
            public int index;
            public FlexLayout mission;

            public Mission(int i, FlexLayout temp)
            {
                index = i;
                mission = temp;
            }
        }
    }
}