﻿namespace RecipeFriends.Shared.DTO;

public class TagInfo: INameable
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
