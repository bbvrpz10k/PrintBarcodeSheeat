using System.Windows;

namespace PrintBarcodeSheeat
{
    public class Settings
    {
        /// <summary>
        /// Ширина сторінки
        /// </summary>
        public double WidthPage { get; set; } = 8.3;
        public double HeightPage { get;  set; } = 11.3;
        public double ListLeftMargin { get;  set; } = 0.14;
        public double ListTopMargin { get; set; } = 0.4;
        public double ElementBorderThickness { get; set; } = 0;
        public double ElementHeight { get; set; } = 75;
        public double ElementWidth { get; set; } = 137;
        public Thickness ElementGridThickness { get; set; } = new Thickness(0, 1, 8, 0);
        public double ElementGridThicknessLeft { get { return ElementGridThickness.Left; } 
            set => ElementGridThickness = new Thickness(value, ElementGridThickness.Top, 
                ElementGridThickness.Right, ElementGridThickness.Bottom);
        }
        public double ElementGridThicknessTop
        {
            get { return ElementGridThickness.Top; }
            set => ElementGridThickness = new Thickness(ElementGridThickness.Left, value,
                ElementGridThickness.Right, ElementGridThickness.Bottom);
        }
        public double ElementGridThicknessRight
        {
            get { return ElementGridThickness.Right; }
            set => ElementGridThickness = new Thickness(ElementGridThickness.Left, ElementGridThickness.Top,
                value, ElementGridThickness.Bottom);
        }
        public double ElementGridThicknessBottom
        {
            get { return ElementGridThickness.Bottom; }
            set => ElementGridThickness = new Thickness(ElementGridThickness.Left, ElementGridThickness.Top,
                ElementGridThickness.Right, value);
        }


    }
}
