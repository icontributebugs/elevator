// See https://aka.ms/new-console-template for more information

class Program
{
    public static void Main(string[] args)
    {
        int elevatorsCount = 1;
        int topFloor = 20;
        
        Elevator[] elevators = new Elevator[elevatorsCount];
        for (int i = 0; i < elevatorsCount; i++)
        {
            elevators[i] = new Elevator(i + 1, topFloor);
        }

        while (true)
        {
            foreach (var elevator in elevators)
            {
                elevator.DoWork();
                
                // Simulate people summoning elevators
                Random random = new Random();
                if (random.NextSingle() < 0.2)
                {
                    int floor = random.Next(0, topFloor + 1);
                    elevator.MoveTo(floor);
                    
                    Console.WriteLine($"Elevator {elevator.GetId()} summoned to floor {floor}");
                }
            }
            
            Thread.Sleep(1000);
        }
    }
}

class Doors
{
    private bool _isOpen;

    public Doors()
    {
        _isOpen = false;
    }

    public bool AreOpen()
    {
        return _isOpen;
    }
    
    public void Open()
    {
        _isOpen = true;
    }

    public void Close()
    {
        _isOpen = false;
    }
}

class Elevator
{
    private int _id;
    private bool _isMoving;
    private bool _isMovingUp;
    private int _position;
    private readonly bool[] _summonedToFloors;
    private readonly Doors[] _doorsAtFloors;

    public Elevator(int id, int topFloor)
    {
        _id = id;
        _isMoving = false;
        _isMovingUp = true;
        _position = 0;
        
        _summonedToFloors = new bool[topFloor + 1];
        Array.Fill(_summonedToFloors, false);
        
        _doorsAtFloors = new Doors[topFloor + 1];
        for (int floor = 0; floor < topFloor + 1; floor++)
        {
            _doorsAtFloors[floor] = new Doors();
        }
    }

    public void MoveTo(int toFloor)
    {
        _summonedToFloors[toFloor] = true;
    }

    public int GetId()
    {
        return _id;
    }

    public void DoWork()
    {
        if (!_isMoving)
        {
            HandleIdling();
        }
        else
        {
            HandleMovement();
        }
    }

    private void HandleIdling()
    {
        if (_doorsAtFloors[_position].AreOpen())
        {
            _doorsAtFloors[_position].Close();
            
            Console.WriteLine($"Elevator {_id} closed doors at floor {_position}");
        }
        else
        {
            ScheduleMovement();
        }
    }

    private void HandleMovement()
    {
        int movementDelta = _isMovingUp ? 1 : -1;
        _position += movementDelta;

        Console.WriteLine($"Elevator {_id} moved to floor {_position}");
        
        if (_summonedToFloors[_position])
        {
            _summonedToFloors[_position] = false;
            _isMoving = false;
            Console.WriteLine($"Elevator {_id} opens doors at floor {_position}");
            _doorsAtFloors[_position].Open();
        }
    }

    private void ScheduleMovement()
    {
        if (ShouldMove(_isMovingUp))
        {
            _isMoving = true;
        }
        else if (ShouldMove(!_isMovingUp))
        {
            _isMoving = true;
            _isMovingUp = !_isMovingUp;
        }
        else
        {
            _isMoving = false;
        }
    }

    private bool ShouldMove(bool isMovingUp)
    {
        int movementDelta = isMovingUp ? 1 : -1;
        Func<int, bool> shouldContinueLoop = isMovingUp
            ? i => i < _summonedToFloors.Length
            : i => i >= 0;
        
        for (int position = _position + movementDelta; shouldContinueLoop(position); position += movementDelta)
        {
            if (_summonedToFloors[position])
            {
                return true;
            }
        }

        return false;
    }
}