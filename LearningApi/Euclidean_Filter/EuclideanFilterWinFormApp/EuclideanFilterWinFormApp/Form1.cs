using System;
using System.Drawing;
using System.Windows.Forms;
using LearningFoundation;
using EuclideanFilter;

namespace EuclideanFilterWinFormApp
{
    public partial class Form1 : Form
    {
        private Bitmap _original;

        public Form1()
        {
            InitializeComponent();
            showCurrentValueRedLabel.Text = "0";
            showCurrentValueGreenLabel.Text = "0";
            showCurrentValueBlueLabel.Text = "0";
            showCurrentValueRadiusLabel.Text = "0";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public static double[,,] ConvertFromBitmapToDoubleArray(Bitmap bitmap)
        {
            int imgWidth = bitmap.Width;
            int imgHeight = bitmap.Height;

            //0 -> R, 1 -> G, 2 -> B
            double[,,] imageArray = new double[imgWidth, imgHeight, 3];

            for (int i = 0; i < imgWidth; i++)
            {
                for (int j = 0; j < imgHeight; j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    imageArray[i, j, 0] = color.R;
                    imageArray[i, j, 1] = color.G;
                    imageArray[i, j, 2] = color.B;
                }

            }

            return imageArray;
        }

        public static Bitmap ConvertFromDoubleArrayToBitmap(double[,,] imageArray)
        {
            int imgWidth = imageArray.GetLength(0);
            int imgHeight = imageArray.GetLength(1);

            //0 -> R, 1 -> G, 2 -> B
            Bitmap bitmap = new Bitmap(imgWidth, imgHeight);

            for (int i = 0; i < imgWidth; i++)
            {
                for (int j = 0; j < imgHeight; j++)
                {
                    Color color = GetAndSetPixels.GetPixel(imageArray, i, j);
                    bitmap.SetPixel(i, j, color);
                }
            }

            return bitmap;
        }

        private void DisplayImage(string filename)
        {
            _original = new Bitmap(filename);

            originalPictureBox.Image = _original;
        }

        private void choosePictureButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                choosePictureTextBox.Text = openFileDialog.FileName;
                DisplayImage(openFileDialog.FileName);
            }
        }

        private void FilterAndDisplayResult(Bitmap originalBitmap)
        {
            if (originalBitmap == null)
                return;

            int red = redTrackBar.Value;
            int green = greenTrackBar.Value;
            int blue = blueTrackBar.Value;
            float radius = radiusTrackBar.Value;

            LearningApi api = new LearningApi();
            EuclideanFilterModule module = new EuclideanFilterModule(Color.FromArgb(255, red, green, blue), radius);
            double[,,] ConvertedBitmapImg = ConvertFromBitmapToDoubleArray(originalBitmap);

           double[,,] filteredImg = module.Run(ConvertedBitmapImg, null);

            Bitmap filteredImgAsBitmap = ConvertFromDoubleArrayToBitmap(filteredImg);

            filteredPictureBox.Image = filteredImgAsBitmap;
        }

        private void updatePictureWithNewValuesButton_Click(object sender, EventArgs e)
        {
            updatePictureWithNewValuesButton.Enabled = false;
            FilterAndDisplayResult(_original);
            updatePictureWithNewValuesButton.Enabled = true;
        }

        private void redTrackBar_Scroll(object sender, EventArgs e)
        {
            showCurrentValueRedLabel.Text = redTrackBar.Value.ToString();
        }

        private void greenTrackBar_Scroll(object sender, EventArgs e)
        {
            showCurrentValueGreenLabel.Text = greenTrackBar.Value.ToString();
        }

        private void blueTrackBar_Scroll(object sender, EventArgs e)
        {
            showCurrentValueBlueLabel.Text = blueTrackBar.Value.ToString();
        }

        private void radiusTrackBar_Scroll(object sender, EventArgs e)
        {
            showCurrentValueRadiusLabel.Text = radiusTrackBar.Value.ToString();
        }
    }
}
