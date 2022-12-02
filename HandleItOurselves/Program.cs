
Console.WriteLine("Don't use a Policy approach");

    try
    {
        bool successful = false;
        int attempts = 0;

        while (!successful)
        {
            try
            {
                attempts++;
                DoSomething();
                successful = true;
            }
            catch (Exception e)
            {
                if (attempts > 1)
                {
                    throw;
                }
            }
        }
    }
    catch
    {
        Console.WriteLine("Doing thing errored");
    }


void DoSomething()
{
    Console.WriteLine("Doing Thing");
    throw new Exception();
}