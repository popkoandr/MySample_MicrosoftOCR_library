using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// not my realization of PhotosPreview control

namespace App1.Carousel
{
    public sealed partial class PhotosPreview : UserControl
    {
        // image width
        private static double IMG_WIDTH = 255;

        //image height
        private static double IMG_HEIGHT = 144;

        //spring speed used for animation
        private static double SPRINESS = 0.4;

        // bounce speed used for animation
        private static double DECAY = 0.5;

        // scale between images
        private static double SCALE_DOWN_FACTOR = 0.5;

        // distance between images
        private static double OFFSET_FACTOR = 150;

        // opacity between images
        private static double OPACITY_DOWN_FACTOR = 0.4;

        // max scale value
        private static double MAX_SCALE = 2.0;

        private double _xCenter;
        private double _yCenter;

        // target moving position. this value shows the value of the centered image
        private double _target = 0;

        // current pos
        private double _current = 0;

        // temp used to store last moving
        private double _spring = 0;

        //save added images
        private List<Image> _images = new List<Image>();

        // fps of the on enter frame event
        private static int FPS = 22;

        // timer used to animate images
        private DispatcherTimer _timer = new DispatcherTimer();
        
        public PhotosPreview()
        {
            this.InitializeComponent();

            //find center position
            _xCenter = WorkAreaGrid.Width / 2;
            _yCenter = WorkAreaGrid.Height / 2;
        }

        public static DependencyProperty ListImagesAsPictureCarouselProperty =
            DependencyProperty.Register("ListImagesAsPictureCarousel", typeof(List<Image>), typeof(PhotosPreview), new PropertyMetadata(null));
        public List<Image> ListImagesAsPictureCarousel
        {
            get
            {
                return (List<Image>)GetValue(ListImagesAsPictureCarouselProperty);

            }
            set
            {
                SetValue(ListImagesAsPictureCarouselProperty, value);
                //st visability to visiable -> this how up all controls on UI

                MainGridControl.Visibility = Visibility.Visible;

                AddImagesToCarousel();

                //set position to center of images
                SetIndex(_images.Count / 2);
                Start();
            }
        }

        private void Start()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / FPS);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object sender, object e)
        {
            for (int i = 0; i < _images.Count; i++)
            {
                Image image = _images[i];
                SetPosImage(image, i);
            }

            // added animation effect
            if (_target == _images.Count)
                _target = 0;
            _spring = (_target - _current) * SPRINESS + _spring * DECAY;
            _current += _spring;
        }

        private void SetIndex(int value)
        {
            _target = value;
            _target = Math.Max(0, _target);
            _target = Math.Min(_images.Count - 1, _target);
        }

        private void AddImagesToCarousel()
        {
            if (ListImagesAsPictureCarousel != null && ListImagesAsPictureCarousel.Count > 0)
            {
                for (int i = 0; i < ListImagesAsPictureCarousel.Count; i++)
                {
                    //TAG is used as an identificator for images
                    ListImagesAsPictureCarousel[i].Tag = i.ToString();


                    //event tapped to find selected image
                    ListImagesAsPictureCarousel[i].Tapped += image_Tapped;

                    LayoutRoot.Children.Add(ListImagesAsPictureCarousel[i]);
                    SetPosImage(ListImagesAsPictureCarousel[i], i);
                    _images.Add(ListImagesAsPictureCarousel[i]);
                }
            }

        }

        private void SetPosImage(Image image, int index)
        {
            double diffFactor = index - _current;

            // adapt scale and position for each image based on position and scale factors
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = MAX_SCALE - Math.Abs(diffFactor) * SCALE_DOWN_FACTOR;
            scaleTransform.ScaleY = MAX_SCALE - Math.Abs(diffFactor) * SCALE_DOWN_FACTOR;
            image.RenderTransform = scaleTransform;

            // position image
            double left = _xCenter - (IMG_WIDTH * scaleTransform.ScaleX) / 2 + diffFactor * OFFSET_FACTOR;
            double top = _yCenter - (IMG_HEIGHT * scaleTransform.ScaleY) / 2;
            image.Opacity = 1 - Math.Abs(diffFactor) * OPACITY_DOWN_FACTOR;

            image.SetValue(Canvas.LeftProperty, left);
            image.SetValue(Canvas.TopProperty, top);

            image.SetValue(Canvas.ZIndexProperty, (int)Math.Abs(scaleTransform.ScaleX * 100));
        }

        private void image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image image = (Image)sender;
            // need implementation
        }

        private void button_left_Click(object sender, RoutedEventArgs e)
        {
            MoveIndex(-1);
        }

        private void button_right_Click(object sender, RoutedEventArgs e)
        {
            MoveIndex(1);
        }

        private void MoveIndex(int value)
        {
            _target += value;
            _target = Math.Max(0, _target);
            _target = Math.Min(_images.Count - 1, _target);
        }
    }
}
