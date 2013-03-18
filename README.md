RestfulRouting-Redirect
=======================

Using Restful Routing, we can redirect old urls to new ones to keep urls COOL

This works great, and I didn't have to modify the Core of RestfulRouting. Sometimes you find that you want to change your urls to something else. Curse you vanity!!!

Anyways, what you need to do is know what the old Url is, and what the "new" resource is.

So you need to keep the old routes registered along side the new routes, and then you can daisy chain off of them. If that doesn't make sense, just look at the sample. This is an initial spike but works great.

    // register the old url, and tell it the values to the new resource
    // we will use a UrlHelper to look into the RouteCollection and
    // hydrate the new Url
    map.Redirect("shops")
       .WithName("old_boutiques_index")
       .To(new { controller = "shops", action = "index" })
       .GetOnly();


    // We then have to register the global attribute, to intercept
    // any and all RedirectRoutes, very easy
    filters.Add(new RedirectRouteFilter());

Check out the implementation in this repository. Let me know what you think, I'm interested.
