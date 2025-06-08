using System;
using TestBucket.Domain.Testing.Heuristics.Models;

namespace TestBucket.Domain.Testing.Heuristics.Defaults;
internal class DefaultHeuristics
{
    internal static Heuristic[] Defaults => [
        new Heuristic ()
            {
                Name = "Goldilocks",
                Description = """
                # Too Big, Too Small, Just Right

                Consider data entry fields and test with data entries that are too big, too small or with entries that are not suitable for the input such as
                entering a string in a numeric field.
               
                """
            },

        new Heuristic()
        {
            Name = "CRUD",
            Description = """
            # Create, Read, Update, Delete
            """
        },

        new Heuristic
        {
            Name = "CQRS",
            Description = """
            Command Query Responsibility Segregation
            """
        },

        new Heuristic()
        {
            Name = "RCRCRC (Karen N. Johnson)",
            Description = """
            Recent - what testing around new areas of code should I think about?

            Core - what essential functions or features must continue to work?

            Risky - what features or areas of code are inherently more risky?

            Configuration Sensitive - what code is dependent on environment settings? 

            Repaired - what code has changed to address defects and potentially created issues?

            Chronic - what code typically breaks features that need to be tested?
            """
        },

        new Heuristic
        {
            Name = "Data Scenarios",
            Description = """
            Perform a sequence of actions involving data, verifying the data integrity at each step. (Example: Enter → Search → Report → Export → Import → Update → View)
            """
        },

        new Heuristic()
        {
            Name = "Numbers",
            Description = """
            Consider:
            - Consider numbers that may cross common integer boundaries, such as 256, 65536, 65537, 2147483648, 2147483649, 4294967296, 4294967297
            - Negative numbers
            - Zero
            - Internationalization and number formats when presented in the UI
            """
        },
        new Heuristic()
        {
            Name = "Internationalization",
            Description = """
            Consider:
            - Translations
            - Sort order
            - Decimal character could be a comma (,) or a period (.) etc
            - Thousands separator could be (empty), a space, a comma (,) or a period (.) etc
            - Some cultures may have a separator at ten-thousands instead of thousands
            - Colors can have different meaning in different cultures
            """
        },
        new Heuristic()
        {
            Name = "Strings",
            Description = """
            - Special characters such as /\*,.<>|\\()[]{};:`!@#$%^&* for text input fields (including username, email and password fields). These characters could be used in different combinations, and in combination with normal alpha numerical latin characters.
            - Different length of strings.
            - Accented characters (àáâãäåçèéêëìíîðñòôõöö, etc.) and emojis.
            - Non-latin scripts such as simplified chinese, traditional chinese, thai, arabic, cyrillic scripts.
            - SQL injection patterns.
            - HTML input
            """
        },
        new Heuristic()
        {
            Name = "BINMEN",
            Description = """
            Boundary, Invalid Entries, NULL, Method, Empty, Negativ
            """
        },

        new Heuristic()
        {
            Name = "POSIED",
            Description = """
            Parameters, Output, Interop, Security, Errors, Data
            """
        },

        new Heuristic()
        {
            Name = "VADER",
            Description = """
            Verbs, Authorisation/Authentication, Data, Errors, Responsiveness
            """
        },

        new Heuristic()
        {
            Name = "Mobile UI",
            Description = """
            - Mobile Device 
            - Mobile OS/Platform
            - Orientation
            - Interrupts and Integractions
            - Battery Usage
            - Slow network / High Latency
            - No network
            """
        },

        new Heuristic()
        {
            Name = "Files and Paths",
            Description = """
            - Special characters in name (space * ? / \ | < > , . ( ) [ ] { } ; : ‘ “ ! @ # $ % ^ &  )
            - Illegal filename, e.g. COM1 in windows
            - Unicode characters, including Emoji
            - File does not exist
            - File already exists
            - Long filename
            - Long paths (deep folder hierarchy)
            - Folder already exists
            - Out of disk space
            - Minimal Space
            - No write access
            - Unavailable
            - File is locked
            - File is located on a network share
            - File is located in cloud storage
            - File is corrupted
            - File has no file extension
            - File has the wrong file extension
            - File is temporarily locked by another process or by anti-virus software
            """
        },

          new Heuristic ()
            {
                Name = "Time and Date",
                Description = """
                Consider various input to date and time fields or properties.
                Consider changing the globalization/internationalization options of the system used to test the product.

                - Date/time entry fields or presentation of date/time and different date formats, time formats (24h or am/pm).
                - Internationalization 
                - Daylight saving
                - Leap year (febuary 29), and invalid date inputs.
                
                For multiple dates, consider using different time zones if that can be selected.
                """
            },

            new Heuristic ()
            {
                Name = "Performance",
                Description = """
                - That the action completes in a suitable time frame
                - For actions that take a long time to execute, a loading indicator should be displayed
                """
            },


            new Heuristic ()
            {
                Name = "Resource Utilization",
                Description = """
                - That the action doesn't use an unreasonable amount of CPU
                - That the action doesn't use an unreasonable amount of GPU
                - That the action doesn't use an unreasonable amount of RAM
                - That the action doesn't use an unreasonable amount of storage space
                """
            },
        ];
}
