using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace вычмат_6лаба
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                float x0 = (float)numericUpDown1.Value;
                float y0 = (float)numericUpDown2.Value;
                float xn = (float)numericUpDown3.Value;
                float h = (float)numericUpDown4.Value;

                List<int> index = new List<int>();
                foreach (int indexChecked in checkedListBox1.CheckedIndices) //смотрим какой график выбрал пользователь
                {
                    index.Add(indexChecked);
                }
                ChartForm chart = new ChartForm(index, x0, y0, xn, h);
                chart.Show();

            }
            catch
            {
                MessageBox.Show("Ошибка!");
            }
        }
    }

    class ChartForm : Form
    {
        public float function(float x, float y)
        {
            float f = x * x - x + y;
            return f;
        }

        public float sollution(float x)
        {
            return (float)Math.Exp(x) - x * x - x - 1;
        }

        public float Euler(float x, float y, float h)
        {
            float yn = y + h * function(x, y);
            return yn;
        }

        public float Euler2(float x, float y, float h)
        {
            float yn = y + h * (function(x, y) + function(x + h, y + h * function(x, y))) / 2;
            return yn;
        }

        public float Runge(float x, float y, float h)
        {
            float e = (float)Math.Pow(10, -5);
            while (true)
            {
                float k1 = h * function(x, y);
                float k2 = h * function(x + h / 3, y + k1 / 3);
                float k3 = h * function(x + h / 3, y + k1 / 6 + k2 / 6);
                float k4 = h * function(x + h / 2, y + k1 / 8 + 3 * k3 / 8);
                float k5 = h * function(x + h, y + k1 / 2 - 3 * k3 / 2 + 2 * k4);
                

                float delta = 1 / 30 * (2 * k1 - 9 * k3 + 8 * k4 - k5);
                if (Math.Abs(delta) >= e) h /= 2;
                if (Math.Abs(delta) <= e / 32) h *= 2;
                
                float yn = y + k1 / 6 + 2 * k4 / 3 + k5 / 6;
                
                return yn;
            }
        }

        public float Adams(float x, float y, float h, float[] F)
        {
            float yn = y + h / 24 * (55 * F[3] - 59 * F[2] + 37 * F[1] + 9 * F[0]);
            return yn;
        }

        public ChartForm(List<int> type, float x0, float y0, float xn, float h)
        {
            this.Size = new Size(800, 600);
            //создаем элемент Chart
            Chart myChart = new Chart();
            //кладем его на форму и растягиваем на все окно.

            myChart.Parent = this;
            myChart.Dock = DockStyle.Fill;
            //добавляем в Chart область для рисования графиков
            myChart.ChartAreas.Add(new ChartArea("Math functions"));
            //Создаем и настраиваем набор точек для рисования графика
            myChart.Legends.Add(new Legend("MyLegend"));

            Series euler = new Series();
            euler.ChartType = SeriesChartType.Line;
            euler.ChartArea = "Math functions";
            euler.LegendText = "Метод Эйлера";
            euler.BorderWidth = 12;

            Series runge = new Series();
            runge.ChartType = SeriesChartType.Line;
            runge.ChartArea = "Math functions";
            runge.Color = Color.Red;
            runge.LegendText = "Метод Рунге–Кутты–Мерсона";
            runge.BorderWidth = 8;

            Series euler2 = new Series();
            euler2.ChartType = SeriesChartType.Line;
            euler2.ChartArea = "Math functions";
            euler2.Color = Color.Black;
            euler2.LegendText = "Исправленный Эйлера";
            euler2.BorderWidth = 4;

            Series adams = new Series("");
            adams.ChartType = SeriesChartType.Line;
            adams.ChartArea = "Math functions";
            adams.Color = Color.Gray;
            adams.LegendText = "Адамса 4-го порядка";
            adams.BorderWidth = 2;

            Series sol = new Series("");
            sol.ChartType = SeriesChartType.Line;
            sol.ChartArea = "Math functions";
            sol.Color = Color.Pink;
            sol.LegendText = "Точное решение";
            sol.BorderWidth = 16;

            float yn = y0;

            for (float xi = x0; xi <= xn; xi += h)
            {
                sol.Points.AddXY(xi, sollution(xi));
            }

            myChart.Series.Add(sol);

            foreach (int i in type)
            {
                for (float xi = x0; xi <= xn; xi += h) //подставляем х в нужную функцию и строим график
                {
                    if (i == 0)
                    {
                        if (xi == x0) yn = y0;
                        else
                        { yn = Euler(xi, yn, h); }
                        euler.Points.AddXY(xi, yn); 

                    }

                    if (i == 1)
                    {
                        if (xi == x0) yn = y0;
                        else
                        { yn = Runge(xi, yn, h); }
                        runge.Points.AddXY(xi, yn); 

                    }

                    if(i == 2)
                    {
                        if (xi == x0) yn = y0;
                        else
                        { yn = Euler2(xi, yn, h); }
                        euler2.Points.AddXY(xi, yn);
                    }
                }

                if (i == 3)
                {
                    float[] F = new float[4];
                    F[0] = function(x0, y0);
                    F[1] = function(x0 + h, Euler(x0, y0, h));
                    F[2] = function(x0 + 2 * h, Euler(x0 + h, Euler(x0, y0, h), h));
                    F[3] = function(x0 + 3 * h, Euler(x0 + 2 * h, Euler(x0 + h, Euler(x0, y0, h), h), h));

                    yn = Euler(x0 + 3 * h, Euler(x0 + h, Euler(x0, y0, h), h), h) + h / 24 * (55 * F[3] - 59 * F[2] + 37 * F[1] - 9 * F[0]);

                    for (float xi = x0 + 4 * h; xi <= xn; xi+=h)
                    {
                        F[0] = F[1]; F[1] = F[2]; F[2] = F[3];
                        F[3] = function(xi, yn);
                        yn +=  h / 24 * (55 * F[3] - 59 * F[2] + 37 * F[1] - 9 * F[0]);
                        adams.Points.AddXY(xi, yn);
                    }
                }

                if (i == 0) myChart.Series.Add(euler);
                if (i == 1) myChart.Series.Add(runge);
                if (i == 2) myChart.Series.Add(euler2);
                if (i == 3) myChart.Series.Add(adams);
            }
            
        }
    }
}


