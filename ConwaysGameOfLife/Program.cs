const char ALIVE = 'X';
const char DEAD = ' ';

Console.CursorVisible = false;
var rows = Console.WindowHeight;
var columns = Console.WindowWidth;
var state = new char[rows][];
var buffer = new char[rows][];
var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));

for (var i = 0; i < rows; i++)
{
    state[i] = new char[columns];
    buffer[i] = new char[columns];
}

while (true)
{
    var seed = GetSeedFromUser();
    Initialize(state, seed);

    while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
    {
        PlayRound(state, buffer);

        await timer.WaitForNextTickAsync();
        Render(state, buffer);
    }
}

static int GetSeedFromUser()
{
    Console.Clear();
    Console.WriteLine("Enter a seed value:");    
    while (true)
    {
        var input = Console.ReadLine();
        if (int.TryParse(input, out var seed))
        {
            Console.Clear();
            return seed;
        }
        Console.WriteLine("Invalid input. Please enter a valid number.");
    }
}

static void Initialize(char[][] state, int seed)
{
    var random = new Random(seed);
    for (var i = 0; i < state.Length; i++)
    {
        for (var j = 0; j < state[i].Length; j++)
        {
            var value = random.Next(0, 2) == 1 ? ALIVE : DEAD;
            state[i][j] = value;
            Console.SetCursorPosition(j, i);
            Console.Write(value);
        }
    }
}

static void PlayRound(char[][] state, char[][] buffer)
{
    for (var i = 0; i < state.Length; i++)
    {
        for (var j = 0; j < state[i].Length; j++)
        {
            var neighbors = 0;
            for (var y = i - 1; y <= i + 1; y++)
            {
                for (var x = j - 1; x <= j + 1; x++)
                {
                    if (x == j && y == i)
                        continue;

                    if (y < 0 || y >= state.Length || x < 0 || x >= state[i].Length)
                        continue;

                    if (state[y][x] == ALIVE)
                        neighbors++;
                }
            }

            buffer[i][j] = neighbors switch
            {
                2 => state[i][j],
                3 => ALIVE,
                _ => DEAD
            };
        }
    }
}

static void Render(char[][] state, char[][] buffer)
{
    for (var i = 0; i < state.Length; i++)
    {
        for (var j = 0; j < state[i].Length; j++)
        {
            if (state[i][j] != buffer[i][j])
            {
                state[i][j] = buffer[i][j];
                Console.SetCursorPosition(j, i);
                Console.Write(state[i][j]);
            }
        }
    }
}
