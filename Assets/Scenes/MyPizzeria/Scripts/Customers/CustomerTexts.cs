using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.AI.Customers
{
    public class CustomerTexts : MonoBehaviour
    {
        public static string[] GreetingsText = new string[]
        {
            "Hi",
            "Hello",
            "Good Morning, sir",
            "Oh, hi!",
            "Greetings!",
            "Good Morning",
            "Uh... Hi!",
            "Hmm..."

        };
        public static string[] noCashRegister = new string[]
        {
            "Uhhh...",
            "Where can I order my pizza?",
            "I want to eat something...",
            "Hello? Anyone? I want to eat...",
            "Nowhere to buy pizza... hmmm..."
        };
        public static string[] NormalGiveUpLeave = new string[]
        {
            "Huh... weird...",
            "I guess I'll just leave...",
            "Looks like no pizza for me...",
            "Maybe the rumours were real...",
            "I'm gonna eat somewhere else...",
            "Why did nobody sell me pizza?",
            "No pizzas? :("

        };
        public static string[] noChairs = new string[]
{
            "uhhh... Where do I sit?",
            "No seats?",
            "Oh... every seat is taken...",
            "Okay, great... Now where do I sit?",
            "Can't wait to get my Pizza... wait... am I supposed to sit on the floor?",
            "Looks like no sitting for me...",
            "I don't know where to sit...",
            "Consider buying yourself more chairs, or tables, or I don't know...",
            "Even if there would be any empty seat, I would need a table to eat."

};
        public static string[] noChairPath = new string[]
{
            "I... can't reach the chair...",
            "How am I supposed to get there?",
            "Something is in my way... Please help me...",
            "I can't get to that chair!",
            "There is no way I'm sitting there...",
            "I can't pass through..."

};
        public static string[] OtherChair = new string[]
{
            "I'll just find another seat...",
            "Nevermind... I'll find another seat...",
            "Hmm... another seat I guess?",
            "Nah, another seat...",
            "I think I'm going to sit somewhere else."

};
        public static string[] OrderText = new string[]
                {
            "I would like to order \"%pizzaname%\" pizza.",
            "I want \"%pizzaname%\" pizza.",
            "Could you make me \"%pizzaname%\" pizza? I really like it!",
            "I think I'll take... uhhh... \"%pizzaname%\" pizza... yeahh... \"%pizzaname%\"!",
            "Hmmmm... Let me think... I think... It will be \"%pizzaname%\" pizza...",
            "I'm going to order \"%pizzaname%\".",
            "\"%pizzaname%\" Pizza for me.",
            "I will take \"%pizzaname%\" Pizza.",
            "\"%pizzaname%\" sounds amazing!",
            "Do you serve \"%pizzaname%\"? Amazing! I'll take it!",
            "I'm starving, what about \"%pizzaname%\"",
            "\"%pizzaname%\" Pizza please...",
            "What about \"%pizzaname%\"?",
            "\"%pizzaname%\" is my choice :)",
            "\"%pizzaname%\".",
        };
        public static string[] OrderGiveUpText = new string[]
                {
            "Well... let me see... I think there's actually nothing interesting. Welp...",
            "Wait... I don't think I'm actually taking anything...",
            "Uhhh... nevermind...",
            "I don't see anything I'd like to eat... Sorry!",
            "Sorry, your menu lacks pizza I like... That's... Unfortunate...",
            "No... Wait... You don't have anything I like...",
            "I must admit, that's not the biggest choice I've ever seen... I think I'm just going to pass on that. Maybe next time!"
        };
        public static string[] PatienceGiveUp = new string[]
        {
            "How much am I supposed to wait!?",
            "That's it. Goodbye.",
            "Are you kidding me? I've been waiting for too long!",
            "I don't care, I'm leaving.",
            "You will regret making me wait for so long!",
            "I won't wait any second longer.",
            "Are you serious? Goodbye!",
            "Do I seriously have to wait for so long?"
};
        public static string[] DirtyText = new string[]
{
            "What is it supposed to be? I don't want it! Bring me another!",
            "No, I will not eat it.",
            "This pizza is dirty! Ew!",
            "Ew... Disgusting, why is it so dirty?",
            "Have you ever washed your hands?!",
            "It's so dirty! Is this a joke!?",
            "Are you serious? Why is it so dirty?!",
            "Have you been using this pizza as a brooom?!"
};    
        public static string[] BadGiveUp = new string[]
{
            "That's it. I'm leaving.",
            "No, I will not eat it. Goodbye.",
            "Goodbye.",
            "I am leaving.",
};
        public static string[] PatiencePizzaGiveUp = new string[]
{
            "How much am I supposed to wait!?",
            "That's it. Goodbye.",
            "Are you kidding me? I've been waiting for too long!",
            "I don't care, I'm leaving.",
            "You will regret making me wait for so long!",
            "I won't wait any second longer.",
            "Are you serious? Goodbye!",
            "Do I seriously have to wait for so long?"
};
        public static string[] noSeatsGiveup = new string[]
                {
            "That's it. I'm leaving.",
            "I'm not paying for this. Goodbye.",
            "Why are you even taking orders if there are no seats?",
            "Ugh... I don't have time for this. I'm leaving",
            "I want you to cancel my order.",
            "Buy yourself new chairs! Goodbye."

        };
        public static string[] timeShorterPizzeriaClosed = new string[]
        {
            "Oh, it's already closed... And I'm still here",
            "It got late, my patience is running out. Where's my Pizza?",
            "Thanks for letting me stay for a bit longer, however I want to get my Pizza already.",
            "Closing time already? You better get moving with my order!",
            "Where's my Pizza?",
            "I want to leave, but first I want my Pizza."
};
        public static string[] EmptyPlate = new string[]
{
            "Yes. I see that plate... What about it?",
            "An empty... plate?",
            "A plate. What about it?",
            "Yes I see it, are you going to bring me my Pizza?",
            "Where's my Pizza?",
            "An empty plate?",
            "Why are you showing me an empty plate?",
            "Why are you showing this to me?",
            "I'm waiting for my Pizza."
};
        public static string[] NoPlate = new string[]
{
            "Am I gonna to just place it on the table?",
            "Where's the plate?",
            "Am I supposed to place it on the floor? Get me a plate...",
            "I could use a plate.",
            "...\n...\na plate?"
};
        public static string[] Under10Bake = new string[]
{
            "Are you sure it's baked enough?",
            "Please check if it's baked enough again",
            "I can already see it probably wasn't even put in the oven",
            "What? You forgot something...",
            "Are you kidding me? I'm not gonna bake it at home!",
            "Nice joke, now bake it."
};
        public static string[] BurntText = new string[]
        {
            "What is this, exactly?",
            "Are you selling me coal? Go make a new one! Quickly!",
            "Is this a joke?",
            "You think I'm going to eat it?",
            "Is this some kind of a prank?",
            "I'm not gonna eat it. Make me a new one.",
            "What kind of stupid idea this is?",
            "Nobody would eat it!"
        };
        public static string[] NotCut = new string[]
{
            "Well looks like I have to cut it myself.",
            "Where's the knife... oh, there it is...",
            "Oh, you didn't cut it?",
            "You forgot about something...",
};
        public static string[] WrongPizza = new string[]
{
            "This is not what I ordered...",
            "I didn't order it!",
            "What is that? I didn't ask for this!",
            "I don't think this is what I ordered.",
            "This is surely not something I asked for.",
            "WHAT?! This isn't what I ordered!!"
};
        public static string[] QuitStaring = new string[]
                {
            "Stop staring at me.",
            "Leave me alone, or I'm going to call the Police.",
            "Leave me alone!",
            "What are you looking at?",
            "Please stop freaking me out",
            "You're creepy...",
            "Could you please just leave me alone?",
            "Stop looking at me like that!",

        };
        public static string[] TooExpensiveGiveUp = new string[]
        {
            "You think I'm going to pay that much for Pizza?",
            "What are these prices? What the hell?",
            "Your Pizza is too expensive. I only have %money%...",
            "Oh, I wish I could buy something, but it's all so expensive! I only have %money%$...",
            "I only have %money%$...",
            "Let me check my wallet... Hmmm... %money%$... Oh... Looks like I'm not ordering anything...",
            "This is way too much as for a Pizza in a Pizzeria like this!",
            "No no no, these prices are way too high.",
            "No way, I'm not paying that much!",
            "These prices are a pure joke!",
            "What did I expect if I went to a Pizzeria with only %money% bucks..."


};
        public static string RandomLine(string[] lines)
        {
            if (lines.Length > 0)
            {
                int w = Random.Range(0, lines.Length);
                return lines[w];
            }
            else
            {
                return "I... don't remember what I actually wanted to say...";
            }
        }
    }
}
