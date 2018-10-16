using System;
using System.Collections.Generic;

namespace LibForColorLines
{
    struct Point
    {
        public int X;
        public int Y;
    }
    public class Engine
    {
        private int _sizeOfField; //Переменная для размера поля
        private int _score = 0; //Переменная для подсчета очков

        //Метод устанавливает размер поля//
        public void SetSizeOfField(int sizeOfField)
        {
            _sizeOfField = sizeOfField;
        }

        //Метод возвращает размер поля//
        public int GetSizeOfField()
        {
            return _sizeOfField;
        }

        //Метод устанавливает счет//
        public void SetScore(int score)
        {
            _score = score;
        }

        //Метод возвращает текущий счет//
        public int GetScore()
        {
            return _score;
        }

        //Метод проверяет игровое поле на заполненность//
        public bool FieldIsFull(int[,] field)
        {
            byte count = 0;
            bool flag = false; //Предполагаем, что поле не пустое

            for (int i = 0; i < _sizeOfField; i++)
            {
                for (int j = 0; j < _sizeOfField; j++)
                {
                    if (field[i, j] == 0) count++;
                    if (count > 3) break;
                }
                if (count > 3) break;
            }
            if (count <= 3) flag = true; //Если кол-во пустых клеток не превышает 3-х, меняем значение флага

            return flag;
        }

        //Метод устанавливает три шарика на игровое поле//
        public void SetupBalls(int[,] field)
        {
            Random rand = new Random();
            int posI, posJ, color;
            bool flag;

            for (int i = 0; i < 3; i++) //Устанавливаем три шарика
            {
                if (FieldIsFull(field) == false) //Кривая реализация отслеживания свободных клеток клеток.
                {
                    //Цикл для установки шарика
                    do
                    {
                        flag = false;
                        //Задаем точку установки шарика рандомно по ширине и высоте
                        posI = rand.Next(0, _sizeOfField); //Размер поля = числу. Адрес считается от нуля. Поэтому, граничный адрес ячейки = размер поля.
                        posJ = rand.Next(0, _sizeOfField);
                        //Задаем цвет шарика
                        color = rand.Next(1, 8);
                        //Проверяем, можно ли туда поставить шарик?
                        if (field[posI, posJ] == 0)
                        {
                            field[posI, posJ] = color;
                            flag = true;
                        }
                    }
                    while (flag == false); //Пока флаг = false, цикл выполняется
                }//!
                else break;//!
            }
        }

        //-------------------------------------------------------------------------------------//
        //------------------Блок проверки возможности перемещения шарика-----------------------//
        //-------------------------------------------------------------------------------------//
        private static bool WayIsClear(int[,] field, Point from, Point to)
        {
            var hash = new HashSet<Point>();
            //
            var temp = field[from.Y, from.X];
            field[from.Y, from.X] = 0;
            //
            var res = WayIsClear(field, from, to, hash);
            //
            field[from.Y, from.X] = temp;
            //
            return res;
        }

        private static bool WayIsClear(int[,] field, Point from, Point to, HashSet<Point> hash)
        {
            if (hash.Contains(from)) return false;
            if (!WithInField(field, from)) return false;
            if (field[from.Y, from.X] != 0) return false;
            if (from.Equals(to)) return true;

            hash.Add(from);

            return WayIsClear(field, new Point() { X = from.X + 1, Y = from.Y }, to, hash) ||
                   WayIsClear(field, new Point() { X = from.X - 1, Y = from.Y }, to, hash) ||
                   WayIsClear(field, new Point() { X = from.X, Y = from.Y + 1 }, to, hash) ||
                   WayIsClear(field, new Point() { X = from.X, Y = from.Y - 1 }, to, hash);
        }

        static bool WithInField(int[,] field, Point p)
        {
            return p.X >= 0 && p.X < field.GetLength(1) && p.Y >= 0 && p.Y < field.GetLength(0);
        }
        //-------------------------------------------------------------------------------------//
        //-------------------------------------Конец блока-------------------------------------//
        //-------------------------------------------------------------------------------------//     

        //Метод перемещает шар в место, указанное пользователем. Возвращает true, если шар был перемещен//
        public bool MoveBall(int[,] field, int fromX, int fromY, int toX, int toY)
        {
            bool flag = false;
                //Вызываем метод проверки пути. Передаем ему координаты -1, потому ячейки массива нумеруются с нуля
                if (WayIsClear(field, new Point() { X = fromX - 1, Y = fromY - 1 }, new Point() { X = toX - 1, Y = toY - 1 }) == true)
                {
                    field[toY - 1, toX - 1] = field[fromY - 1, fromX - 1]; //Копируем шарик в нужную клетку
                    field[fromY - 1, fromX - 1] = 0; //Затираем шарик
                    flag = true;
                }
            return flag;
        }

        //Метод ищет линии из пяти одинаковых шаров на игровом поле и убирает их. Возвращает true, если удалил линию//
        public bool SearchLines(int[,] field)
        {
            bool linesWasDeleted = false;
            //Поиск по горизонтали
            for (int i = 0; i < _sizeOfField; i++)
            {
                for (int j = 0; j < _sizeOfField - 4; j++) //Перебираем строчку
                {
                    if ((field[i, j] != 0) && (field[i, j] == field[i, j + 1]) && (field[i, j + 1] == field[i, j + 2]) && (field[i, j + 2] == field[i, j + 3]) && (field[i, j + 3] == field[i, j + 4])) //Проверяем линию из 5 шаров. Никогда. НИКОГДА ТАК НЕ ДЕЛАЙ!
                    {
                        if ((j < _sizeOfField - 5) && (field[i, j + 4] == field[i, j + 5])) //А если линия больше? Проверим. 6 шаров
                        {
                            if ((j < _sizeOfField - 6) && (field[i, j + 5] == field[i, j + 6])) //7 шаров
                            {
                                field[i, j + 6] = 0; //Убираем седьмой шар
                                this._score += 1;
                            }
                            field[i, j + 5] = 0; //Убираем шестой шар
                            this._score += 1;
                        }
                        field[i, j] = field[i, j + 1] = field[i, j + 2] = field[i, j + 3] = field[i, j + 4] = 0; //Убираем 5 шаров с поля. Господи, какой ужас!
                        this._score += 5;
                        linesWasDeleted = true;
                    }
                }
            }

            //Поиск по вертикали
            for (int i = 0; i < _sizeOfField; i++)
            {
                for (int j = 0; j < _sizeOfField - 4; j++) //Перебираем строчку
                {
                    if ((field[j, i] != 0) && (field[j, i] == field[j + 1, i]) && (field[j + 1, i] == field[j + 2, i]) && (field[j + 2, i] == field[j + 3, i]) && (field[j + 3, i] == field[j + 4, i])) //Проверяем линию из 5 шаров. Никогда. НИКОГДА ТАК НЕ ДЕЛАЙ!
                    {
                        if ((j < _sizeOfField - 5) && (field[j + 4, i] == field[j + 5, i])) //А если линия больше? Проверим. 6 шаров
                        {
                            if ((j < _sizeOfField - 6) && (field[j + 5, i] == field[j + 6, i])) //7 шаров
                            {
                                field[j + 6, i] = 0; //Убираем седьмой шар
                                _score += 1;
                            }
                            field[j + 5, i] = 0; //Убираем шестой шар
                            _score += 1;
                        }
                        field[j, i] = field[j + 1, i] = field[j + 2, i] = field[j + 3, i] = field[j + 4, i] = 0; //Убираем 5 шаров с поля. Господи, какой ужас!
                        _score += 5;
                        linesWasDeleted = true;
                    }
                }
            }

            //Поиск по главной диагонали (\)
            for (int i = 0; i < _sizeOfField - 4; i++)
            {
                for (int j = 0; j < _sizeOfField - 4; j++) //Перебираем строчку
                {
                    if ((field[i, j] != 0) && (field[i, j] == field[i + 1, j + 1]) && (field[i + 1, j + 1] == field[i + 2, j + 2]) && (field[i + 2, j + 2] == field[i + 3, j + 3]) && (field[i + 3, j + 3] == field[i + 4, j + 4])) //Проверяем линию из 5 шаров. Никогда. НИКОГДА ТАК НЕ ДЕЛАЙ!
                    {
                        field[i, j] = field[i + 1, j + 1] = field[i + 2, j + 2] = field[i + 3, j + 3] = field[i + 4, j + 4] = 0; //Убираем 5 шаров с поля. Господи, какой ужас!
                        _score += 5;
                        linesWasDeleted = true;
                    }
                }
            }

            //Поиск по побочной диагонали (/) 
            for (int i = 0; i < _sizeOfField - 4; i++)
            {
                for (int j = _sizeOfField - 1; j > 3; j--) //Перебираем строчку. Минус один, не забываем
                {
                    if ((field[i, j] != 0) && (field[i, j] == field[i + 1, j - 1]) && (field[i + 1, j - 1] == field[i + 2, j - 2]) && (field[i + 2, j - 2] == field[i + 3, j - 3]) && (field[i + 3, j - 3] == field[i + 4, j - 4])) //Проверяем линию из 5 шаров. Никогда. НИКОГДА ТАК НЕ ДЕЛАЙ!
                    {
                        field[i, j] = field[i + 1, j - 1] = field[i + 2, j - 2] = field[i + 3, j - 3] = field[i + 4, j - 4] = 0; //Убираем 5 шаров с поля. Господи, какой ужас!
                        _score += 5;
                        linesWasDeleted = true;
                    }
                }
            }
            return linesWasDeleted;
        }

    }
}
