using System;
using System.Data;
using Npgsql;

class Program
{
    static void Main ()
    {
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=Employees;";

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();


            /* 
                 1. Отобразить реквизиты сотрудников, менеджеры которых устроились на работу в 2023 г.,
                 но при это сами эти работники устроились на работу до 2023 г.
            */
            Console.WriteLine("1. Отобразить реквизиты сотрудников, менеджеры которых устроились на работу в 2023 г.,\r\n но при это сами эти работники устроились на работу до 2023 г.");
            Output(
                "SELECT e.FIRST_NAME, e.LAST_NAME, e.HIRE_DATE " +
                "FROM EMPLOYEES e " +
                "JOIN EMPLOYEES m ON e.MANAGER_ID = m.EMPLOYEE_ID " +
                "WHERE m.HIRE_DATE BETWEEN '2023-01-01' AND '2023-12-31' " +
                "AND e.HIRE_DATE < '2023-01-01'; ",
                connection
                );


            /* 
                 2. Отобразить данные по сотрудникам: из какого департамента и какими текущими
                задачами они занимаются.
                Результат отобразить в трех полях: employees.First_name, jobs.Job_title,
                departments.Department_name
            */
            Console.WriteLine("2. Отобразить данные по сотрудникам: из какого департамента и какими текущими\r\n                задачами они занимаются.\r\n                Результат отобразить в трех полях: employees.First_name, jobs.Job_title,\r\n                departments.Department_name");
            Output(
                "SELECT e.FIRST_NAME, j.JOB_TITLE, d.DEPARTMENT_NAME " +
                "FROM EMPLOYEES e " +
                "JOIN JOBS j ON e.JOB_ID = j.JOB_ID " +
                "JOIN DEPARTMENTS d ON e.DEPARTMENT_ID = d.DEPARTMENT_ID ",
                connection
                );


            /* 
                 3. Отобразить город, в котором сотрудники в сумме зарабатывают меньше всех.
            */
            Console.WriteLine("3. Отобразить город, в котором сотрудники в сумме зарабатывают меньше всех.");
            Output(
                "SELECT l.CITY " +
                "FROM EMPLOYEES e " +
                "JOIN DEPARTMENTS d ON e.DEPARTMENT_ID = d.DEPARTMENT_ID " +
                "JOIN LOCATIONS l ON d.LOCATION_ID = l.LOCATION_ID " +
                "GROUP BY l.CITY " +
                "ORDER BY SUM(e.SALARY) ASC " +
                "LIMIT 1; ",
                connection
                );


            /* 
                 4. Вывести все реквизиты сотрудников менеджеры которых устроились на работу в январе
                месяце любого года и длина job_title этих сотрудников больше 15ти символов
            */
            Console.WriteLine("4. Вывести все реквизиты сотрудников менеджеры которых устроились на работу в январе\r\nмесяце любого года и длина job_title этих сотрудников больше 15ти символов");
            Output(
                "SELECT *, j.JOB_TITLE " +
                "FROM EMPLOYEES e " +
                "JOIN JOBS j ON e.JOB_ID = j.JOB_ID " +
                "WHERE (SELECT EXTRACT(MONTH FROM e.HIRE_DATE)) = 1 " +
                "AND LENGTH(j.JOB_TITLE) > 15; ",
                connection
                );


            /* 
                 5. Вывести реквизит first_name сотрудников, которые живут в Europe (region_name)
            */
            Console.WriteLine("5. Вывести реквизит first_name сотрудников, которые живут в Europe (region_name)");
            Output(
                "SELECT e.FIRST_NAME " +
                "FROM EMPLOYEES e " +
                "JOIN DEPARTMENTS d ON e.DEPARTMENT_ID = d.DEPARTMENT_ID " +
                "JOIN LOCATIONS l ON d.LOCATION_ID = l.LOCATION_ID " +
                "JOIN COUNTRIES c ON c.COUNTRY_ID = l.COUNTRY_ID " +
                "JOIN REGIONS r ON r.REGION_ID = c.REGION_ID " +
                "WHERE r.REGION_NAME = 'Europe'; ",
                connection
                );


            /* 
                 6. Получить детальную информацию о каждом сотруднике:
                First_name, Last_name, Departament, Job, Street, Country, Region
            */
            Console.WriteLine("6. Получить детальную информацию о каждом сотруднике:\r\nFirst_name, Last_name, Departament, Job, Street, Country, Region");
            Output(
                "SELECT " +
                    "e.FIRST_NAME, " +
                    "e.LAST_NAME, " +
                    "d.DEPARTMENT_NAME AS Department, " +
                    "j.JOB_TITLE AS Job, " +
                    "l.STREET_ADDRESS AS Street, " +
                    "c.COUNTRY_NAME AS Country, " +
                    "r.REGION_NAME AS Region " +
                "FROM EMPLOYEES e " +
                "JOIN DEPARTMENTS d ON e.DEPARTMENT_ID = d.DEPARTMENT_ID " +
                "JOIN LOCATIONS l ON d.LOCATION_ID = l.LOCATION_ID " +
                "JOIN COUNTRIES c ON c.COUNTRY_ID = l.COUNTRY_ID " +
                "JOIN REGIONS r ON r.REGION_ID = c.REGION_ID " +
                "JOIN JOBS j ON e.JOB_ID = j.JOB_ID; ",
                connection
                );


            /* 
                 7. Отразить регионы и количество сотрудников в каждом из них.
            */
            Console.WriteLine("7. Отразить регионы и количество сотрудников в каждом из них.");
            Output(
                "SELECT r.REGION_NAME, (COUNT(e.EMPLOYEE_ID)) " +
                "FROM EMPLOYEES e " +
                "JOIN DEPARTMENTS d ON e.DEPARTMENT_ID = d.DEPARTMENT_ID " +
                "JOIN LOCATIONS l ON d.LOCATION_ID = l.LOCATION_ID " +
                "JOIN COUNTRIES c ON c.COUNTRY_ID = l.COUNTRY_ID " +
                "JOIN REGIONS r ON r.REGION_ID = c.REGION_ID " +
                "GROUP BY r.REGION_NAME; ",
                connection
                );


            /* 
                 8. Вывести информацию по департаменту department_name с минимальной и максимальной
                зарплатой, с ранней и поздней датой прихода на работу и с количеством сотрудников.
                Сортировать по количеству сотрудников (по убыванию)
            */
            Console.WriteLine("8. Вывести информацию по департаменту department_name с минимальной и максимальной\r\nзарплатой, с ранней и поздней датой прихода на работу и с количеством сотрудников.\r\nСортировать по количеству сотрудников (по убыванию)");
            Output(
                "SELECT " +
                    "d.DEPARTMENT_NAME AS Department, " +
                    "MIN(e.SALARY) AS MinSalary, " +
                    "MAX(e.SALARY) AS MaxSalary, " +
                    "MIN(e.HIRE_DATE) AS EarliestHireDate, " +
                    "MAX(e.HIRE_DATE) AS LatestHireDate, " +
                    "COUNT(e.EMPLOYEE_ID) AS EmployeeCount " +
                "FROM EMPLOYEES e " +
                "JOIN DEPARTMENTS d ON e.DEPARTMENT_ID = d.DEPARTMENT_ID " +
                "GROUP BY d.DEPARTMENT_NAME " +
                "ORDER BY EmployeeCount DESC; ",
                connection
                );


            /* 
                 9. Получить список реквизитов сотрудников FIRST_NAME, LAST_NAME и первые три цифры от
                номера телефона, если номер в формате ХХХ.ХХХ.ХХХХ
            */
            Console.WriteLine("9. Получить список реквизитов сотрудников FIRST_NAME, LAST_NAME и первые три цифры от\r\nномера телефона, если номер в формате ХХХ.ХХХ.ХХХХ");
            Output(
                "SELECT " +
                    "FIRST_NAME, " +
                    "LAST_NAME, " +
                    "LEFT(PHONE_NUMBER, 3) AS PHONE_PREFIX " +
                "FROM EMPLOYEES " +
                "WHERE PHONE_NUMBER LIKE '___.___.____'; ",
                connection
                );


            /* 
                 10. Вывести список сотрудников, которые работают в департаменте администрирования
                доходов (departments.department_name = &#39;DAD&#39;)
            */
            Console.WriteLine("9. Получить список реквизитов сотрудников FIRST_NAME, LAST_NAME и первые три цифры от\r\nномера телефона, если номер в формате ХХХ.ХХХ.ХХХХ");
            Output(
                "SELECT e.FIRST_NAME, e.LAST_NAME, e.JOB_ID, e.SALARY " +
                "FROM EMPLOYEES e " +
                "JOIN DEPARTMENTS d ON e.DEPARTMENT_ID = d.DEPARTMENT_ID " +
                "WHERE d.DEPARTMENT_NAME = 'DAD'; ",
                connection
                );

        }


        // Функция вывода результата запроса в виде таблицы
        void Output (string queryText, NpgsqlConnection connection)
        {
            using (var query = new NpgsqlCommand(queryText, connection))
            {
                using (var reader = query.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // Формируем строку "Название: Значение" для всех столбцов одной строки
                            string columnData = "";

                            // Проходим по всем столбцам в строке
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                // Добавляем отформатированную строку с данными
                                columnData += $"{reader.GetName(i)}: {reader.GetValue(i),-15} | ";  // Выравнивание значений по ширине
                            }

                            // Получаем длину строки
                            int columnWidth = columnData.Length;

                            // Выводим верхний разделитель
                            Console.WriteLine(new string('-', columnWidth));

                            // Выводим данные с рамками
                            Console.WriteLine($"| {columnData}");

                            // Выводим нижний разделитель
                            Console.WriteLine(new string('-', columnWidth));

                        }
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                }
            }
        }
    }
}
