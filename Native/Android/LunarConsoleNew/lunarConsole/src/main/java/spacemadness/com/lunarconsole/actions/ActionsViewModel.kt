package spacemadness.com.lunarconsole.actions

import spacemadness.com.lunarconsole.reactive.combineLatest

class ActionsViewModel(
    actions: ActionRegistry,
    variables: VariableRegistry
) {
    val items =
        combineLatest(actions.itemsStream, variables.itemsStream) { result ->
            val output = mutableListOf<ListItem>()

            // actions
            addListItems("Actions", result[0], output)

            // variables
            addListItems("Variables", result[1], output)

            // result
            output as List<ListItem>
        }

    private fun addListItems(title: String, items: List<Item>, output: MutableList<ListItem>) {
        if (items.isEmpty()) {
            return
        }

        output.add(GroupItem(title, collapsed = false))

        val groups = LinkedHashMap<String, MutableList<ListItem>>()
        items.forEach { item ->
            val listItem = createListItem(item)
            val group = item.group ?: ""
            var groupList = groups[group]
            if (groupList == null) {
                groupList = mutableListOf(listItem)
                groups[group] = groupList
            } else {
                groupList.add(listItem)
            }
        }

        groups.forEach { (group, list) ->
            if (group.isNotEmpty()) {
                output.add(GroupItem(group, collapsed = false))
            }
            output.addAll(list)
        }
    }

    private fun createListItem(item: Item) = when (item) {
        is Action -> ActionItem(item)
        is Variable<*> -> VariableItem(item)
        else -> throw IllegalArgumentException("Unexpected item: $item")
    }
}