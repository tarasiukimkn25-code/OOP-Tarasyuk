using System;

class Teacher
{
    
    private string name;
    private string subject;
    private int experience;

      public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public string Subject
    {
        get { return subject; }
        set { subject = value; }
    }

    public int Experience
    {
        get { return experience; }
        set 
        { 
            if (value >= 0)
                experience = value; 
        }
    }

    
    public Teacher(string name, string subject, int experience)
    {
        this.name = name;
        this.subject = subject;
        Experience = experience; 
    }

    public void Introduce()
    {
        Console.WriteLine($"Вітаю! Мене звати {name}, я викладаю {subject}. Мій стаж: {experience} р.");
    }
}

class Program
{
    static void Main()
    {
        Teacher teacher = new Teacher("Іван Миколайович", "Математика", 10);

        teacher.Introduce();
    }
}

