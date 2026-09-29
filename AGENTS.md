# Feature development workflow

When a prompt asks you to add a new feature to this repository when on the main branch:

1. Parse the prompt and identify the requested behavior, scope, constraints, and any assumptions or open questions.
2. Before starting implementation, create a GitHub issue that records the original prompt and explains how you interpreted it, including the planned scope and any assumptions. Keep the prompt distinguishable from your interpretation. Share the issue link in the chat.
3. Create and switch to a new branch for the feature work. Use the issue number in the branch name when available, and branch from the appropriate base branch without discarding existing work.
4. Once work has started, post a comment on the GitHub issue whenever you give a progress update in the chat. The comment should reflect the substance of that update, including relevant progress, decisions, blockers, or verification results. Keep the issue updated through the final chat summary as well.

# Pull request review comments

When responding to comments on a pull request, analyze each comment against the code and the intended behavior. If a suggestion does not make sense, explain why in the review thread; if its meaning or expected behavior is unclear, ask the commenter for clarification rather than making an unsupported assumption.

When changing a pull request in response to comments, verify and commit the changes to the pull request branch, reply to the relevant comments with what changed, and tag the users who commented to ask them to re-review.

# Documentation

When adding, updating, or deleting a feature, update the documentation to reflect the change.
