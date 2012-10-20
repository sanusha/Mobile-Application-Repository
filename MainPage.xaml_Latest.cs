// ***************************************************************************************************************


// Author: Anusha Swaminathan                                                                                    *
// University of Washington, Bothell MSCSS                                                                       *
// OCt 1 to Oct 20                                                                                               *
// A time line centric hypermedia audio visual system                                                            *
// This project is a novel approach to link audio files, video files and images together to connect related data *
                                                                                                                 
// This is the initial set of code which tells how a page in this app may look with connected artifacts          *
// (video files, audio files and images) along with transcripts for the media files                              *

// Multiple other classes will be created of this similar type for a different gerne connectecing many other     *
// artifacts together.                                                                                           *

// This class manily consists of multiple event handlers to handle any kinds of events in c#                     *


// ***************************************************************************************************************
 

namespace VideoFile
{
    using System;
    using System.Net;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Shapes;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using System.Windows.Documents;
    using System.Windows.Resources;         // for streamResourceInfo
    using Microsoft.Phone.Controls;
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;
    using Microsoft.Xna.Framework.Media;
    using System.Windows.Media.Animation;
    using System.Text.RegularExpressions;
    using System.Windows.Controls.Primitives;

    // this class is the one which includes the bare minimum functionality a app needs
    public partial class MainPage : PhoneApplicationPage
    {
        // Initial set of variables
        DispatcherTimer myDispatcherTimer; // to start the dispatch timer
        bool isClicked = false;      // returns true or false based on the button click
        bool updatingMediaTimeline;
        ImageBrush imageToShow;
        BitmapImage img;
        Point translationRef;
        double scaleRef = 1f;
        Point deltaVec;

        // for embedding YouTube videos
        string url = "";
        string link = ""; 
        
        #region CONSTANTS

        private const string BUTTER_IMAGE = "Butter.png";
        private const string NOKIA_LUMIA_VIDEO = "NokiaLumia.wmv.wmv";
        private const string PROFTALK_VIDEO = "video.wmv";
        private const string EDUSPEECH_VIDEO = "Educational Speech.wmv";

        #endregion

        // Constructor
        public MainPage()
        {
            this.myDispatcherTimer = new DispatcherTimer();
            this.img = new BitmapImage();
            this.translationRef = new Point(0, 0);
            this.deltaVec = new Point(0, 0);
            this.InitializeComponent();
            this.updatingMediaTimeline = false;

            // set the timer value for the video that is being played
            this.myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);

            // Event used for handling the timer control for the seek bar with respect to the media element
            this.myDispatcherTimer.Tick += new EventHandler(setTimeControl);

            // This event handler gets called every second. So based on the functionality of callpopUps
            // method the eevnt is triggered and displays the 
            this.myDispatcherTimer.Tick += new EventHandler(callPopUps);
            this.myDispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            this.myDispatcherTimer.Start();

            // for YouTube videos
            /*WebClient client = new WebClient();
            url = "http://www.youtube.com/watch?v=HLQqOpILDcI";
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ClientDownloadStringCompleted);
            client.DownloadStringAsync(new Uri(url, UriKind.Absolute));
            Uri uri = new Uri(url);
            mediaElement1.Source = uri;*/

        }


        #region MEDIA_SEEKBAR_INTEGRATION

        // function to get the current play time of the video
        // This method uses a delegates and lambda expression 
        // to set the value of the seek bar based on the video content

        private void setTimeControl(object o, EventArgs sender)
        {
            CompositionTarget.Rendering += (s, e) =>
                {
                    this.updatingMediaTimeline = true;
                    var duration = this.mediaElement1.NaturalDuration.TimeSpan;
                    if (duration.TotalSeconds != 0)
                    {
                        double percentComplete = this.mediaElement1.Position.TotalSeconds / duration.TotalSeconds;
                        this.seekBar.Value = percentComplete;
                        TimeSpan mediaTime = this.mediaElement1.Position;
                        this.current.Text = String.Format("{0:00}:{1:00}:{2:00}", this.mediaElement1.Position.Hours,
                                                this.mediaElement1.Position.Minutes, this.mediaElement1.Position.Seconds);
                        this.updatingMediaTimeline = false;
                    }
                };
        }

        #endregion

        # region Total Video Play Time

        // function to get the total play time of the video
        // this function deals with "DownloadProgresssChanged" event in MediaElement
        private void downloadProgressHandler(object sender, RoutedEventArgs e)
        {
            this.total.Text = String.Format("{0:00}:{1:00}:{2:00}", this.mediaElement1.NaturalDuration.TimeSpan.Hours,
                                this.mediaElement1.NaturalDuration.TimeSpan.Minutes, 
                                    this.mediaElement1.NaturalDuration.TimeSpan.Seconds);
        }

        # endregion

        # region VideoaudioStop

        // stop button to stop the video. just include the StopMedia in click in xaml for MediaElement
        private void StopMedia(object sender, RoutedEventArgs e)
        {
            this.mediaElement1.Stop();
        }

        #endregion


        # region VideoAudioPause

        // pause button to pause the video. just include the PauseMedia in click in xaml for MediaElement
        private void PauseMedia(object sender, RoutedEventArgs e)
        {
            this.mediaElement1.Pause();
        }

        #endregion


        #region VideoAudioPlay

        // play button to play the video. just include the PlayMedia in click in xaml for MediaElement
        private void PlayMedia(object sender, RoutedEventArgs e)
        {
            this.mediaElement1.Play();
        }

        #endregion

        // this method contains the actions to be done when popUpImage 2 fails
        private void image2_ImageFailed(object sender, System.Windows.RoutedEventArgs e)
        {

        }


        private void mediaOpened(object sender, RoutedEventArgs e)
        {
            if (this.mediaElement1.NaturalDuration.HasTimeSpan)
            {
                var ts = this.mediaElement1.NaturalDuration.TimeSpan;
                this.seekBar.Maximum = ts.TotalSeconds;
                this.seekBar.SmallChange = 1;
                this.seekBar.LargeChange = Math.Min(10, ts.Seconds / 10);
                this.seekBar.Value = this.mediaElement1.Position.TotalSeconds;
            }
            this.seekBar.Maximum = this.mediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds;
            this.seekBar.Minimum = this.mediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds;
        }


        private void mouseclick(object sender, MouseEventArgs e)
        {
            this.isClicked = true;
        }

        private void mouseUnClick(object sender, MouseEventArgs e)
        {
            this.isClicked = false;
            this.mediaElement1.Position = TimeSpan.FromSeconds(this.seekBar.Value);
        }

        void timer_Tick(object Sender, EventArgs e)
        {
            if (!this.isClicked)
            {
                this.seekBar.Value = this.mediaElement1.Position.TotalSeconds;
            }

        }

        // This method is used to set the position of the media element based
        // on the time span.
        private void mediaTimeline_Valuechanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!this.updatingMediaTimeline && this.mediaElement1.CanSeek)
            {
                var duration = this.mediaElement1.NaturalDuration.TimeSpan;
                int newPosition = (int)(duration.TotalSeconds * seekBar.Value);
                this.mediaElement1.Position = new TimeSpan(0, 0, newPosition);
            }
        }

        // This is a button event which when triggered performs the necessery actions
        // moving the transcriptionpara1 to the desired position
        private void MoveToDesiredPosition(object sender, RoutedEventArgs e)
        {
            // when the button is clicked, the corresponding video moves to the position pointed
            // by the transcript. This also drags the seek bar automatically to that position.
            this.mediaElement1.Position = new TimeSpan(0, 1, 12);

            // changing the color of the text at run time.
            // this is mnaily used to change the transcript's textcolor in the particular paragraph whe the button
            // containing the transcript is clicked.
            this.transcriptionPara1.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 142, 35));
        }


        #region PopUpRegion

        // This function calls the corresponding images when the seekbar reaches 
        // a corresponding time.
        void callPopUps(object o, EventArgs sender)
        {
            if (this.mediaElement1.Source == new Uri(PROFTALK_VIDEO, UriKind.Relative))
            {
                if (this.mediaElement1.Position.Seconds == 10)
                {
                    this.popUpImage1.Visibility = Visibility.Visible;
                }

                if (this.mediaElement1.Position.Minutes == 1 && this.mediaElement1.Position.Seconds == 10)
                {
                    this.popUpImage2.Visibility = Visibility.Visible;
                    this.popUpImage1.Visibility = Visibility.Collapsed;
                }


                if (this.mediaElement1.Position.Minutes == 1 && this.mediaElement1.Position.Seconds == 40)
                {
                    this.popUpImage2.Visibility = Visibility.Collapsed;
                }
            }
            // if the content in the media element is " ", the corresponding
            // sequence of actions take place
            else if (this.mediaElement1.Source == new Uri(NOKIA_LUMIA_VIDEO, UriKind.Relative))
            {
                var uri = new Uri(BUTTER_IMAGE, UriKind.Relative);
                var imgSource = new BitmapImage(uri);

                if (this.mediaElement1.Position.Seconds == 10)
                {
                    this.popUpImage1.Source = imgSource;
                    this.popUpImage1.Visibility = Visibility.Visible;
                }
            }
            // if the content in the media element is " ", the corresponding
            // sequence of actions take place
            else if (this.mediaElement1.Source == new Uri(EDUSPEECH_VIDEO, UriKind.Relative))
            {
                // TODO: Actions based on video content
            }
        }


        #endregion

        #region PopUpImage2

        // event handler for popUpImage1..........................................
        // when the first pop up image is selected the following actions take place
        // based on the content that is played inside the media player
        private void popUpImage1Selected(object sender, MouseEventArgs e)
        {
            if (this.mediaElement1.Source == new Uri(PROFTALK_VIDEO, UriKind.Relative))
            {
                this.mediaElement1.Source = new Uri(NOKIA_LUMIA_VIDEO, UriKind.Relative);
                this.transcriptionPara1.Content = "welcome to the milky lumiere nine hundred unboxing and first review take a" +
                "look at the device and go over some of the features described not so much an" +
                "offer similar videos" +
                "right now this is gone for a hundred dollars into your contract but with some" +
                "of the connected issues they were having they're getting a hundred dollars back" +
                "knocking on your bill that's great and pick one up" +
                "so let's take a look at the specs of this" +
                "as a four point three x clear black amela displaying" +
                "27but it's eight hundred by four eighty resolution only no issue there" +
                "windows phone seven point five named offering system with the eight megapixel" +
                "cross iceland's" +
                "camera with auto focus so we'll take a look at that later" +
                "bus is going on about that" +
                "so there's the quickstart if you need to know how to start this but" +
                "i'm sure you can figure it out" +
                "and there's a device i have to sign and return here" +
                "a little bit different looking at a black slacks" +
                "sold will put that to the side" +
                "and it does take a micro semiconducting for us so it comes with it" +
                "simple removal kit" +
                "and here's some more people work" +
                "on the looming" +
                "to get the most of the windows phone but we're not going to look at that in each" +
                "week and there's some more documentation here" +
                "barca safety information" +
                "who was destroyed inside" +
                "that pre-taped" +
                "and doesn't look like there's much that comes with the box here is the u_s_ the" +
                "sinking and charging cable"+
                "here is the interesting looking" +
                "usb charger" +
                "and it's nice to circular something different than we've seen before but" +
                "i think that's it for the box there is really" +
                "now feels that comes in the box phones arent thing but" +
                "explains the" +
                "the cheaper contract price so" +
                "let's take a look at this and push them outside" +
                "and here is the device:59and let's go and take the stickers office" +
                "there is that a four point three inch display" +
                "and this device feels very solid so far look at the back off and there's the" +
                "eight megapixel carl zeiss lens with two other the flash" +
                "and it does feel extremely solid in their hand so far it's made out of one" +
                "piece of polycarbonate plastic" +
                "that is colored all the way through the sorties scratched the back he'll scratch of blue" +
                "and there's a closer look" +
                "eight megapixel camera and" +
                "it does have some slight curves" +
                "and there is the speaker that's on the bottom" +
                "and on the right side causal your buttons yet you're dedicated camera fee" +
                "your power on and off" +
                "and your up-and-down rocker points which" +
                "and up top you have the micro semaphore" +
                "which you can see here you go over that tool" +
                "the usb charge important sinking" +
                "secondary mike in a three point five million headphone jack" +
                "so that is it for a bit" +
                "and it feels very good it's madden finished" +
                "so not slippery or anything like that so i like";
                
                this.popUpImage1.Visibility = Visibility.Collapsed;
                this.mediaElement1.Play();
            }

            else if (this.mediaElement1.Source == new Uri(NOKIA_LUMIA_VIDEO, UriKind.Relative))
            {
                this.mediaElement1.Source = new Uri(EDUSPEECH_VIDEO, UriKind.Relative);
                this.transcriptionPara1.Content = "i had never next to the job" +
                "last year julia graduation day" +
                "traineeship and the others hispanic" +
                "sister charity run motivator hastings" +
                "thank you dissolve insisted species ths" +
                "she has so many things administration that" +
                "regional rationing of nightmarish festival" +
                "possibilities salazar in october uh" +
                "teaching children adapt i want to go out";
                this.popUpImage1.Visibility = Visibility.Collapsed;
                this.mediaElement1.Play();
            }
        }


        #endregion


        #region PopUpImage1

        // event handler for popUpImage2..............................................
        // when the second pop up image is selected the following actions take place
        // based on the content that is played inside the media player
        private void popUpImage2Selected(object sender, MouseEventArgs e)
        {
            var uri = new Uri(BUTTER_IMAGE, UriKind.Relative);
            var imgSource = new BitmapImage(uri);
            this.artifaceImage1.Source = imgSource;
            this.artifaceImage1.Visibility = Visibility.Visible;
        }

        #endregion
        
        // pinch manipulation services
        
        #region toolkit gesture event service support
        
        // we only need to keep track of translation 
        // and for translation, we only care about the middle
        // position of the two fingers

        // OK, so the correct translation/scale we want to perform is:
        //
        //    1. move the initial middle of the two finger back to the initial translation
        //    2. scale up by the current scaling
        //    3. move back to the current middle of the two fingers
        //  
        // If, 
        //       if1, if2: are the initial finger touch
        //       translationRef:  is the initial translation
        //       s: is the current scaling factor [this is e.DistanceRatio]
        //              e.DistanceRaio is the currentDistanceBetweenFingers/initDistanceBetweenFingers
        //       cf1, cf1: is the current finger position
        // 
        // what we want is:
        //
        //       im = 0.5 * (if1 + if2)             <-- middle position of the two initial finger position
        //       deltaVec = translationRef - im   <-- translate back by im
        //       cm = 0.5 * (cf1 + cf2)             <-- middle position of the two current finger position
        // 
        // Translation:  cm + (deltaVec * s)
        //   

        private void GestureListener_PinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            Point if1 = e.GetPosition(this.mImageRectangle, 0);      // position of first finger
            Point if2 = e.GetPosition(this.mImageRectangle, 1);      // position of second finger
            Point im = new Point((0.5f * (if1.X + if2.X)),      // middle between the two fingers
                                 (0.5f * (if1.Y + if2.Y)));

            CompositeTransform t = this.imageToShow.Transform as CompositeTransform;
            this.translationRef.X = t.TranslateX;
            this.translationRef.Y = t.TranslateY;
            this.scaleRef = t.ScaleX;  // X and Y scales are always the same in our case

            // deltaVec will take the middle of initial touch position to the 
            // initial translation position. This will give the proper effect of 
            // scaling with respect to the initial touch position
            this.deltaVec.X = this.translationRef.X - im.X;
            this.deltaVec.Y = this.translationRef.Y - im.Y;
        }

        // event to handle gesture support for the object
        private void GestureListener_PinchDelta(object sender, PinchGestureEventArgs e)
        {
            Point cf1 = e.GetPosition(this.mImageRectangle, 0); // current finger 1
            Point cf2 = e.GetPosition(this.mImageRectangle, 1); // current finger 2

            Point cm = new Point((0.5f * (cf1.X + cf2.X)), (0.5f * (cf1.Y + cf2.Y)));

            Point newPos = new Point(cm.X + (this.deltaVec.X * e.DistanceRatio),
                                     cm.Y + (this.deltaVec.Y * e.DistanceRatio));

            CompositeTransform t = imageToShow.Transform as CompositeTransform;
            t.ScaleX = e.DistanceRatio * scaleRef;
            t.ScaleY = e.DistanceRatio * scaleRef;
            t.TranslateX = newPos.X;
            t.TranslateY = newPos.Y;
        }

        // event triggered when gesture- pinch manipulated is completed
        private void GestureListener_PinchCompleted(object sender, PinchGestureEventArgs e)
        {

        }

        #endregion

        #region YOUTUBE VIDEOS EMBEDDING

        // This function is used to embed YouTube videos direct from net
        private void ClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // regex used for a regex split
            Regex rx; 

            // for finding a match
            MatchCollection match;

            rx = new Regex("(?<=url_encoded_fmt_stream_map=)([^(\\\\)]*)(?=\\\\)", RegexOptions.IgnoreCase);
            match = rx.Matches(e.Result);
            string video_format = match[0].ToString();

            // defifing 3 seperators for seperating the url with those values
            string sep1 = "%2C";
            string sep2 = "%26";
            string sep3 = "%3D";

            // slipt the url based on the sep1 and load it into an array
            string[] videoFormatsGroup = Regex.Split(video_format, sep1);

            for (var i = 0; i < videoFormatsGroup.Length; i++)
            {
                // split the obatined string using the othet seperators and put into an array
                string[] videoFormatsElem = Regex.Split(videoFormatsGroup[i], sep2);
                if (videoFormatsElem.Length < 5) continue;
                string[] partialResult1 = Regex.Split(videoFormatsElem[0], sep3);
                if (partialResult1.Length < 2) continue;
                url = partialResult1[1];
                url = HttpUtility.UrlDecode(HttpUtility.UrlDecode(url));
                string[] partialResult2 = Regex.Split(videoFormatsElem[4], sep3);
                if (partialResult2.Length < 2) continue;
                int itag = Convert.ToInt32(partialResult2[1]);
                if (itag == 18)
                {
                    link = url;
                }

                // finally assign the link obtained to the url which will be loaded into the media player
                //url = (string)e.Result;
            }

        }
        #endregion
    }
}
