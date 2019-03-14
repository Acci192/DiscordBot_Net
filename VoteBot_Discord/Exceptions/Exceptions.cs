using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Exceptions
{
    public class PollNotCreated : Exception
    {

    }

    public class UnauthorizedUser : Exception
    {

    }

    public class NoOptionAdded : Exception
    {

    }

    public class UserAlreadyVoted : Exception
    {

    }

    public class InvalidOption : Exception
    {

    }

    public class UserHasNotVoted : Exception
    {

    }

    public class PollAlreadyEnded : Exception
    {

    }
}
