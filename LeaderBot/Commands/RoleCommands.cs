﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace LeaderBot.Commands
{
    [Group("admin")]
    public class RoleCommands : ModuleBase
    {
        public List<Roles> items;
        public Random rand;

        public RoleCommands()
        {
            rand = new Random();
            LoadJson();
        }

        public void LoadJson()
        {
            using (StreamReader r = new StreamReader("roles.json"))
            {
                var json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<Roles>>(json);
            }
        }

        //https://docs.stillu.cc/
        [Command("createRoles"), Summary("Creates a role in the guild")]
        public async Task createRoles()
        {
            List<string> currentGuildRoles = new List<string>();
            foreach (SocketRole guildRoles in ((SocketGuild)Context.Guild).Roles)
            {
                currentGuildRoles.Add(guildRoles.Name);
            }

            foreach (var role in items)
            {
                if (!currentGuildRoles.Contains(role.Name))
                {
                    var randColor = new Color(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
                    await Context.Guild.CreateRoleAsync(role.Name, GuildPermissions.None, randColor);
                    await Logger.Log(new LogMessage(LogSeverity.Verbose , GetType().Name + ".createRoles", "Added role to server: " + role.ToString()));
                }
            }
        }

        [Command("giveRole"), Summary("Adds role to specified user"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task giveRole([Summary("The user to add role to")] SocketGuildUser user, [Summary("The role to add")] string roleName){

            var userInfo = user as SocketUser;
            var currentGuild = user.Guild as SocketGuild;
            var role = currentGuild.Roles.FirstOrDefault(x => x.Name.ToLower() == roleName.ToLower());
            await Logger.Log(new LogMessage(LogSeverity.Info, GetType().Name + ".addRole", userInfo.ToString() + " added role " + role.ToString()));
            await (userInfo as IGuildUser).AddRoleAsync(role);
        }
    }
}
